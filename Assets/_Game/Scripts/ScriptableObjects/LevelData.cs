// TOY WAR RUSH - LevelData.cs
// ScriptableObject for each handcrafted level.

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ToyWarRush/LevelData", fileName = "LevelData_")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string worldName;
    public WorldTheme theme;

    [Header("Starting Conditions")]
    public int startingArmySize = 5;

    [Header("Gates")]
    public List<GateData> gates = new();

    [Header("Obstacles")]
    public List<ObstacleData> obstacles = new();

    [Header("Collectibles")]
    public List<CollectibleData> collectibles = new();

    [Header("Fortress")]
    public int fortressHP = 100;
    public int fortressDefenderCount = 10;
    public bool hasBoss;
    public BossType bossType;

    [Header("Rewards")]
    public int coinsReward = 50;
    public int xpReward = 30;
    public int threeStarRequirement = 80;
    public int twoStarRequirement = 40;
}

[System.Serializable]
public class GateData
{
    public GateOperation operation;
    public float value;
    public float zPosition;
    public float xPosition;
}

[System.Serializable]
public class ObstacleData
{
    public ObstacleType type;
    public float zPosition;
    public float xPosition;
    public int unitDamage;
}

[System.Serializable]
public class CollectibleData
{
    public float zPosition;
    public float xPosition;
    public int armyBonus = 1;
}

public enum WorldTheme { Bedroom, StudyRoom, Classroom, ToyStore, GamingRoom, Kitchen, Garage, Garden, Bathroom, Space }
public enum ObstacleType { Cat, Pencil, Book, Football, Keyboard, SockBall, ToyTrain, RubberDuck, Toolbox, Flowerpot }
public enum BossType { None, BigTeddy, RCCarKing, RoboGeneral, CardboardDragon, TheCat, UltraToyGod }
