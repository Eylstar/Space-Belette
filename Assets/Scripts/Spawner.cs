using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<SpawnWave> spawnWaves = new();
    int currentWave = 0;

    void Start()
    {
        foreach (SpawnWave wave in spawnWaves)
        {
            wave.SpawnRulesInitialisation();
            wave.onSpawn += Spawn;
        }
    }
    
    void Spawn(SpawnRule sr)
    {
        GameObject prefab = GetRandomPrefab(sr.prefabsToSpawn); 
    }

    
    GameObject GetRandomPrefab(List<WeightedPrefab> prefabs)
    {
        float totalWeight = 0;
        foreach (WeightedPrefab wp in prefabs)
        {
            totalWeight += wp.weight;
        }
        
        float randomWeight = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (WeightedPrefab wp in prefabs)
        {
            currentWeight += wp.weight;
            if (randomWeight <= currentWeight)
            {
                return wp.prefab;
            }
        }
        return null;
    }
    
    void OnDisable()
    {
        foreach (SpawnWave wave in spawnWaves)
        {
            wave.onSpawn -= Spawn;
            wave.Disable();
        }
    }
}

[Serializable]
class SpawnWave
{
    public Action<SpawnRule> onSpawn;
    public SpawnRulesSO spawnRules;
    public float delay;
    
    List<SpawnRoutineContainer> spawnRoutineContainers;

    public void SpawnRulesInitialisation()
    {
        spawnRoutineContainers = new();
        foreach (SpawnRule sr in spawnRules.spawnRules)
        {
            SpawnRoutineContainer spawnRoutineContainer = new GameObject("SpawnRoutineContainer").AddComponent<SpawnRoutineContainer>();
            spawnRoutineContainer.StartSpawnRoutine(sr);
            spawnRoutineContainer.onSpawnEvent += onSpawn.Invoke;
            spawnRoutineContainers.Add(spawnRoutineContainer);
        }
    }
    
    public void Disable()
    {
        foreach (SpawnRoutineContainer src in spawnRoutineContainers)
        {
            if (src != null)
            {
                src.onSpawnEvent -= onSpawn.Invoke;
                src.CancelInvoke();
                Object.Destroy(src.gameObject);
            }
        }
        spawnRoutineContainers.Clear();
    }
}

class SpawnRoutineContainer : MonoBehaviour
{
    public Action<SpawnRule> onSpawnEvent;
    SpawnRule currentRule;
    
    public void StartSpawnRoutine(SpawnRule spawnRule)
    {
        currentRule = spawnRule;
        InvokeRepeating(nameof(SpawnCallback), spawnRule.spawnRate, spawnRule.spawnRate);
    }
    
    void SpawnCallback()
    {
        onSpawnEvent?.Invoke(currentRule);
    }
}