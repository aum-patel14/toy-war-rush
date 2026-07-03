// TOY WAR RUSH - LevelGenerator.cs
// Procedural level generation for levels 101+.

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ToyWarRush/ProceduralConfig", fileName = "ProceduralConfig")]
public class ProceduralConfig : ScriptableObject
{
    public int gateChunks = 5;
    public int obstacleChunks = 5;
    public int collectChunks = 3;
    public float chunkLength = 20f;
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private ProceduralConfig config;
    [SerializeField] private List<LevelData> gateChunks = new();
    [SerializeField] private List<LevelData> obstacleChunks = new();

    public static LevelData Generate(int levelNumber)
    {
        var level = ScriptableObject.CreateInstance<LevelData>();
        level.levelNumber = levelNumber;
        level.theme = levelNumber <= 10 ? WorldTheme.Garden
            : levelNumber <= 20 ? WorldTheme.Garage
            : WorldTheme.Space;
        level.worldName = levelNumber <= 10 ? "Desert Highway"
            : levelNumber <= 20 ? "Cave"
            : "Outdoor";
        level.startingArmySize = 4 + levelNumber;
        level.fortressHP = 30 + levelNumber * 12;
        level.fortressDefenderCount = 8 + levelNumber * 2;
        level.hasBoss = levelNumber == 5 || levelNumber == 10 || levelNumber == 15 || levelNumber == 20 || levelNumber == 25 || levelNumber == 30;
        level.coinsReward = 50 + levelNumber * 20;
        level.threeStarRequirement = 75;
        level.twoStarRequirement = 45;

        float z = 40f;
        level.gates.Add(new GateData { operation = GateOperation.Add, value = 5 + levelNumber * 2, zPosition = z, xPosition = 0 });
        z += 55f;
        level.gates.Add(new GateData { operation = GateOperation.Multiply, value = 2, zPosition = z, xPosition = -1f });
        z += 55f;
        if (levelNumber >= 3)
            level.gates.Add(new GateData { operation = GateOperation.Subtract, value = Mathf.Min(3 + levelNumber, 8), zPosition = z, xPosition = 0 });
        z += 55f;
        level.gates.Add(new GateData { operation = GateOperation.Multiply, value = 2, zPosition = z, xPosition = 1f });

        level.obstacles.Add(new ObstacleData { type = ObstacleType.SockBall, zPosition = 70 + levelNumber * 3, xPosition = 1.2f, unitDamage = 2 + levelNumber / 2 });
        level.collectibles.Add(new CollectibleData { zPosition = 100, xPosition = -1.2f, armyBonus = 1 });
        return level;
    }

    public LevelData GenerateLevel(int levelNumber, WorldTheme theme)
    {
        var level = ScriptableObject.CreateInstance<LevelData>();
        level.levelNumber = levelNumber;
        level.theme = theme;
        level.worldName = theme.ToString();
        level.startingArmySize = 10 + levelNumber / 10;
        level.fortressHP = Mathf.RoundToInt(levelNumber * levelNumber * 0.8f);
        level.coinsReward = 50 + levelNumber * 2;

        if (config == null || gateChunks.Count == 0) return level;

        var gateChunk = gateChunks[Random.Range(0, gateChunks.Count)];
        var obstacleChunk = obstacleChunks.Count > 0
            ? obstacleChunks[Random.Range(0, obstacleChunks.Count)]
            : null;

        if (gateChunk != null)
            level.gates.AddRange(gateChunk.gates);

        if (obstacleChunk != null)
            level.obstacles.AddRange(obstacleChunk.obstacles);

        return level;
    }
}
