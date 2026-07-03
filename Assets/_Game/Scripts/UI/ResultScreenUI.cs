// TOY WAR RUSH - ResultScreenUI.cs
// Victory and defeat screens.

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TextMeshProUGUI coinsRewardText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private string mainMenuScene = "MainMenu";

    public void ShowVictory()
    {
        victoryPanel?.SetActive(true);
        defeatPanel?.SetActive(false);

        var levelData = LevelManager.Instance?.CurrentLevelData;
        if (coinsRewardText != null && levelData != null)
            coinsRewardText.text = $"+{levelData.coinsReward}";

        if (starsText != null)
        {
            int stars = LevelManager.Instance?.CalculateStars() ?? 1;
            starsText.text = new string('⭐', stars);
        }

        AudioManager.Instance?.PlaySFX("victory_fanfare");
    }

    public void ShowDefeat()
    {
        victoryPanel?.SetActive(false);
        defeatPanel?.SetActive(true);
    }

    public void OnNextLevel()
    {
        LevelManager.Instance?.LoadNextLevel();
        gameObject.SetActive(false);
    }

    public void OnRetry()
    {
        LevelManager.Instance?.RetryLevel();
        gameObject.SetActive(false);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnWatchAdDoubleCoins()
    {
        AdManager.Instance?.ShowRewardedAd(success =>
        {
            if (success)
                RewardSystem.Instance?.GrantDoubleCoinsReward();
        });
    }

    public void OnWatchAdContinue()
    {
        AdManager.Instance?.ShowRewardedContinue(success =>
        {
            if (success)
            {
                RewardSystem.Instance?.GrantContinueReward();
                gameObject.SetActive(false);
            }
        });
    }
}
