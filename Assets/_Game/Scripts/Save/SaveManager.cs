// TOY WAR RUSH - SaveManager.cs
// Handles local save via JSON + optional cloud sync.

using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameSaveData
{
    public int currentLevel = 1;
    public int coins = 0;
    public int gems = 0;
    public int totalStars = 0;
    public int[] levelStars = new int[500];
    public bool[] unlockedSkins = new bool[50];
    public int[] upgradeLevels = new int[20];
    public int loginStreak = 0;
    public string lastLoginDate = "";
    public long offlineStartTime = 0;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private GameSaveData _saveData;
    private string _savePath;

    public GameSaveData Data => _saveData;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(_savePath, json);
        PlayerPrefs.SetInt("coins", _saveData.coins);
        PlayerPrefs.SetInt("level", _saveData.currentLevel);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            _saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        else
        {
            _saveData = new GameSaveData();
        }
    }

    public void AddCoins(int amount)
    {
        _saveData.coins += amount;
        Save();
        EventBus.Publish(GameEvents.CoinsChanged, _saveData.coins);
    }

    public bool SpendCoins(int amount)
    {
        if (_saveData.coins < amount) return false;
        _saveData.coins -= amount;
        Save();
        EventBus.Publish(GameEvents.CoinsChanged, _saveData.coins);
        return true;
    }

    public void SetLevelStars(int level, int stars)
    {
        int index = level - 1;
        if (index < 0 || index >= _saveData.levelStars.Length) return;

        int previous = _saveData.levelStars[index];
        if (stars > previous)
        {
            _saveData.totalStars += stars - previous;
            _saveData.levelStars[index] = stars;
            Save();
        }
    }

    public void AdvanceLevel(int completedLevel)
    {
        if (completedLevel >= _saveData.currentLevel)
        {
            _saveData.currentLevel = completedLevel + 1;
            Save();
        }
    }

    public int CalculateOfflineReward()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long elapsed = now - _saveData.offlineStartTime;
        elapsed = Math.Min(elapsed, 14400);
        return (int)(elapsed / 60f) * 10;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }

    private void OnApplicationQuit()
    {
        _saveData.offlineStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Save();
    }
}
