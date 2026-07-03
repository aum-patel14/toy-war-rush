// TOY WAR RUSH - GameplaySceneLoader.cs
// Ensures level loads when entering Gameplay from Main Menu (Boot path).

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneLoader : MonoBehaviour
{
    private void Start()
    {
        if (LevelManager.Instance == null) return;

        int lvl = SaveManager.Instance?.Data.currentLevel ?? 1;
        if (LevelManager.Instance.CurrentLevel == lvl && LevelManager.Instance.CurrentLevelData != null)
            return;

        LevelManager.Instance.LoadLevel(lvl);
        GameManager.Instance?.SetState(GameState.Playing);
    }
}
