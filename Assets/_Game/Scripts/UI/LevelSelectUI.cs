// TOY WAR RUSH - LevelSelectUI.cs
// 30-level world map with stars and unlock state.

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Transform gridRoot;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private string gameplayScene = "Gameplay";
    [SerializeField] private int maxLevels = 30;

    private void OnEnable() => BuildGrid();

    public void BuildGrid()
    {
        if (gridRoot == null || levelButtonPrefab == null) return;

        foreach (Transform child in gridRoot)
            Destroy(child.gameObject);

        int unlocked = SaveManager.Instance?.Data.currentLevel ?? 1;

        for (int i = 1; i <= maxLevels; i++)
        {
            int lvl = i;
            var btn = Instantiate(levelButtonPrefab, gridRoot);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                int stars = SaveManager.Instance?.Data.levelStars[lvl - 1] ?? 0;
                string starStr = stars > 0 ? new string('★', stars) : "";
                label.text = $"{lvl}\n{starStr}";
            }

            var button = btn.GetComponent<Button>();
            bool isUnlocked = lvl <= unlocked;
            if (button != null)
            {
                button.interactable = isUnlocked;
                button.onClick.AddListener(() => OnLevelSelected(lvl));
            }
        }
    }

    private void OnLevelSelected(int level)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Data.currentLevel = level;
            SaveManager.Instance.Save();
        }
        SceneManager.LoadScene(gameplayScene);
    }

    public void OnClose() => gameObject.SetActive(false);
}
