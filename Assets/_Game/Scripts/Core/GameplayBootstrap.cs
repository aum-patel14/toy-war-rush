// TOY WAR RUSH - GameplayBootstrap.cs
// Ensures managers exist when testing Gameplay scene directly (without Boot).

using UnityEngine;

public class GameplayBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureManagers()
    {
        if (GameManager.Instance != null) return;

        CreateIfMissing<SaveManager>("SaveManager");
        CreateIfMissing<GameManager>("GameManager");
        CreateIfMissing<AudioManager>("AudioManager");
        CreateIfMissing<AnalyticsManager>("AnalyticsManager");
        CreateIfMissing<AdManager>("AdManager");
        CreateIfMissing<CurrencyManager>("CurrencyManager");
        CreateIfMissing<RewardSystem>("RewardSystem");

        GameManager.Instance?.SetState(GameState.Playing);

        if (LevelManager.Instance != null && ArmyManager.Instance != null)
            LevelManager.Instance.LoadLevel(SaveManager.Instance?.Data.currentLevel ?? 1);
    }

    private static void CreateIfMissing<T>(string name) where T : Component
    {
        if (Object.FindAnyObjectByType<T>() != null) return;
        var go = new GameObject(name);
        go.AddComponent<T>();
    }
}
