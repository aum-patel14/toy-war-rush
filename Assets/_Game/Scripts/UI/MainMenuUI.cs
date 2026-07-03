// TOY WAR RUSH - MainMenuUI.cs
// Main menu with play, shop, settings.

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private string gameplaySceneName = "Gameplay";

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateCurrency;
            UpdateCurrency(CurrencyManager.Instance.Coins);
        }

        AdManager.Instance?.ShowBanner();
        RewardSystem.Instance?.GrantOfflineReward();
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCurrency;
        AdManager.Instance?.HideBanner();
    }

    private void UpdateCurrency(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
        if (gemsText != null && SaveManager.Instance != null)
            gemsText.text = SaveManager.Instance.Data.gems.ToString();
    }

    public void OnPlayPressed()
    {
        AudioManager.Instance?.PlaySFX("button_click");
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnShopPressed() => UIManager.Instance?.ShowShop();
    public void OnUpgradePressed() => UIManager.Instance?.ShowUpgradeMenu();
    public void OnLevelsPressed() => UIManager.Instance?.ShowLevelSelect();
}
