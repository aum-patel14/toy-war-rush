// TOY WAR RUSH - RewardSystem.cs
// Grants level rewards, daily rewards, and offline rewards.

using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GrantLevelReward()
    {
        var levelData = LevelManager.Instance?.CurrentLevelData;
        if (levelData == null) return;

        int stars = LevelManager.Instance.CalculateStars();
        int coinReward = levelData.coinsReward;

        if (stars == 3)
            coinReward = Mathf.RoundToInt(coinReward * 1.5f);

        CurrencyManager.Instance?.AddCoins(coinReward);
    }

    public void GrantDoubleCoinsReward()
    {
        var levelData = LevelManager.Instance?.CurrentLevelData;
        if (levelData == null) return;
        CurrencyManager.Instance?.AddCoins(levelData.coinsReward);
    }

    public int GrantOfflineReward()
    {
        int coins = SaveManager.Instance?.CalculateOfflineReward() ?? 0;
        if (coins > 0)
            CurrencyManager.Instance?.AddCoins(coins);
        return coins;
    }

    public void GrantContinueReward()
    {
        int startSize = LevelManager.Instance?.StartingArmySize ?? 5;
        int reviveCount = Mathf.Max(1, startSize / 2);
        ArmyManager.Instance?.ClearArmy();
        ArmyManager.Instance?.AddUnits(reviveCount);
        GameManager.Instance?.SetState(GameState.Playing);
    }
}
