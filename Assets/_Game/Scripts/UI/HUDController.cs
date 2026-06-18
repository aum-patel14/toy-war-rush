// TOY WAR RUSH - HUDController.cs
// In-game HUD: army count, tier, level, coins.

using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI armyCountText;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinsText;

    private void OnEnable()
    {
        ArmyManager.OnArmyCountChanged += UpdateArmyCount;
        ArmyManager.OnEvolutionTriggered += UpdateTier;
        CurrencyManager.Instance.OnCoinsChanged += UpdateCoins;
        UpdateLevel();
        UpdateCoins(CurrencyManager.Instance?.Coins ?? 0);
    }

    private void OnDisable()
    {
        ArmyManager.OnArmyCountChanged -= UpdateArmyCount;
        ArmyManager.OnEvolutionTriggered -= UpdateTier;
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCoins;
    }

    private void UpdateArmyCount(int count)
    {
        if (armyCountText != null)
            armyCountText.text = count.ToString();
    }

    private void UpdateTier(UnitTier tier)
    {
        if (tierText != null)
            tierText.text = $"TIER: {tier.ToString().ToUpper()}";
    }

    private void UpdateLevel()
    {
        if (levelText != null)
            levelText.text = $"Level {LevelManager.Instance?.CurrentLevel ?? 1}";
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    public void OnPausePressed()
    {
        GameManager.Instance?.SetState(GameState.Paused);
    }
}
