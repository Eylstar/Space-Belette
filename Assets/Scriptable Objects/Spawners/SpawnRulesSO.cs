using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpawnRulesSO", menuName = "Scriptable Objects/SpawnRulesSO")]
public class SpawnRulesSO : ScriptableObject
{
    public List<SpawnRule> spawnRules = new();
}

[Serializable] public struct SpawnRule
{
    public SpawnType spawnType;
    public SpawnArea spawnArea;
    public WaveCompletionType waveCompletionType;
    public int spawnCount;
    public float spawnRate;
    public float waveSecondsDurationOrEnemiesToKill;
    public List<WeightedPrefab> prefabsToSpawn;
}

[Serializable] public struct WeightedPrefab
{
    public GameObject prefab;
    public float weight;
}

public enum SpawnType
{
    Enemy,
    Obstacle,
    Collectible
}

public enum SpawnArea
{
    Front,
    Sides,
    Corners,
    RandomAround
}

public enum WaveCompletionType
{
    Time,
    EnemiesKilled,
    Static
}