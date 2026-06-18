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
