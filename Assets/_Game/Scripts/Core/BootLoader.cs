// TOY WAR RUSH - BootLoader.cs
// Initializes managers and transitions to MainMenu.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private float minLoadTime = 1f;

    private IEnumerator Start()
    {
        float startTime = Time.time;

        // Managers on this GameObject initialize in Awake
        SaveManager.Instance?.Save();
        GameManager.Instance?.SetState(GameState.MainMenu);

        float elapsed = Time.time - startTime;
        if (elapsed < minLoadTime)
            yield return new WaitForSeconds(minLoadTime - elapsed);

        SceneManager.LoadScene(mainMenuScene);
    }
}
