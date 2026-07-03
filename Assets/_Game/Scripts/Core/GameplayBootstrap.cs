// TOY WAR RUSH - GameplayBootstrap.cs
// Ensures managers exist when testing Gameplay scene directly (without Boot).

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureManagers()
    {
        CreateIfMissing<SaveManager>("SaveManager");
        CreateIfMissing<GameManager>("GameManager");
        CreateIfMissing<AudioManager>("AudioManager");
        CreateIfMissing<AnalyticsManager>("AnalyticsManager");
        CreateIfMissing<AdManager>("AdManager");
        CreateIfMissing<CurrencyManager>("CurrencyManager");
        CreateIfMissing<RewardSystem>("RewardSystem");

        TryLoadGameplayLevel();
    }

    private static void TryLoadGameplayLevel()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.name.Contains("Gameplay")) return;

        var levelMgr = Object.FindAnyObjectByType<LevelManager>();
        if (levelMgr == null) return;

        int lvl = SaveManager.Instance?.Data.currentLevel ?? 1;
        levelMgr.LoadLevel(lvl);
        GameManager.Instance?.SetState(GameState.Playing);
    }

    private static void CreateIfMissing<T>(string name) where T : Component
    {
        if (Object.FindAnyObjectByType<T>() != null) return;
        var go = new GameObject(name);
        go.AddComponent<T>();
    }
}
