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
    
    public int spawnCount;
    [UnityEngine.Range(0, 1)] public float spawnDeltaPercentage;
    public float spawnRate;
    public float distanceFromPlayerToSpawn;
    
    public WaveCompletionType waveCompletionType;
    public float waveSecondsDurationOrEnemiesToKill;
    
    [UnityEngine.Range(1, 3)] public float groupDispersion;
    [UnityEngine.Range(0, 1)] public float singleRandomOffset;
    public List<WeightedPrefab> prefabsToSpawn;

    float killOrTimeRemaining;

    public void Init()
    {
        killOrTimeRemaining = waveSecondsDurationOrEnemiesToKill;
    }

    public void UpdateCondition(float f)
    {
        killOrTimeRemaining -= f;
    }
    
    public float GetKillOrTimeRemaining()=> killOrTimeRemaining;
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

public enum WaveCompletionType
{
    Time,
    EnemiesKilled,
    Static
}