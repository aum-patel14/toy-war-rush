// TOY WAR RUSH - CurrencyManager.cs
// Manages coins and gems with save integration.

using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Coins => SaveManager.Instance?.Data.coins ?? 0;
    public int Gems => SaveManager.Instance?.Data.gems ?? 0;

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<int>(GameEvents.CoinsChanged, OnCoinsUpdated);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<int>(GameEvents.CoinsChanged, OnCoinsUpdated);
    }

    public void AddCoins(int amount)
    {
        SaveManager.Instance?.AddCoins(amount);
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount) =>
        SaveManager.Instance != null && SaveManager.Instance.SpendCoins(amount);

    public void AddGems(int amount)
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.Data.gems += amount;
        SaveManager.Instance.Save();
        OnGemsChanged?.Invoke(Gems);
    }

    public bool SpendGems(int amount)
    {
        if (SaveManager.Instance == null || SaveManager.Instance.Data.gems < amount)
            return false;

        SaveManager.Instance.Data.gems -= amount;
        SaveManager.Instance.Save();
        OnGemsChanged?.Invoke(Gems);
        return true;
    }

    private void OnCoinsUpdated(int coins) => OnCoinsChanged?.Invoke(coins);
}
