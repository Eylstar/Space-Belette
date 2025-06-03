using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    GameObject player;
    
    [SerializeField] List<SpawnWave> spawnWaves;
    [SerializeField] List<SpawnWave> optionalSpawnWaves;

    void Start()
    {
        player = FindFirstObjectByType<ShipMove>().gameObject;
        foreach (SpawnWave wave in spawnWaves)
        {
            wave.onSpawn += Spawn;
            wave.SpawnRulesInitialisation();
        }
        foreach (SpawnWave wave in optionalSpawnWaves)
        {
            wave.onSpawn += Spawn;
            wave.SpawnRulesInitialisation();
        }
    }
    
    void Spawn(SpawnRule sr)
    {
        int c = sr.spawnCount + Mathf.RoundToInt(sr.spawnCount * sr.spawnDeltaPercentage * Random.Range(-1f, 1f));
        Debug.Log($"Spawning {c} objects of type {sr.spawnType} with wave completion type {sr.waveCompletionType}");
        GameObject prefab = GetRandomPrefab(sr.prefabsToSpawn);
        
        float angle = Random.Range(0f, 360f);
        float distanceFromPlayer  = sr.distanceFromPlayerToSpawn;
        Vector3 center = new Vector3(player.transform.position.x + distanceFromPlayer * Mathf.Cos(angle * Mathf.Deg2Rad), 0, player.transform.position.z + distanceFromPlayer * Mathf.Sin(angle * Mathf.Deg2Rad));
        
        float objectSize = prefab.GetComponent<Renderer>()?.bounds.size.magnitude ?? 1f;
        float spacing = objectSize * sr.groupDispersion;
        float radius = spacing / (2f * Mathf.Sin(Mathf.PI / c));
        
        for (int i = 0; i < c; i++)
        {
            float angleOffset = i * (360f / c);
            float angleRad = angleOffset * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            
            float maxOffset = sr.singleRandomOffset * spacing;
            Vector2 randomOffset2D = Random.insideUnitCircle * maxOffset;
            Vector3 randomOffset = new Vector3(randomOffset2D.x, 0, randomOffset2D.y);
            Vector3 spawnPosition = center + offset + randomOffset;
            
            Quaternion rotation = sr.spawnType switch
            {
                //Enemy looks at player
                SpawnType.Enemy => Quaternion.LookRotation(player.transform.position - spawnPosition, Vector3.up),
                SpawnType.Obstacle => Quaternion.Euler(0, Random.Range(0f, 360f), 0),
                SpawnType.Collectible or _ => Quaternion.identity
            };
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, rotation);
            spawnedObject.transform.parent = transform;
        }
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
    
    List<SpawnRoutineContainer> spawnRoutineContainers;

    public void SpawnRulesInitialisation()
    {
        spawnRoutineContainers = new List<SpawnRoutineContainer>();
        foreach (SpawnRule sr in spawnRules.spawnRules)
        {
            SpawnRoutineContainer spawnRoutineContainer = new GameObject("SpawnRoutineContainer").AddComponent<SpawnRoutineContainer>();
            if (onSpawn != null)
            {
                spawnRoutineContainer.onSpawnEvent += onSpawn.Invoke;
            }
            else
            {
                Debug.LogWarning("onSpawn is null when subscribing.");
            }
            spawnRoutineContainer.StartSpawnRoutine(sr);
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