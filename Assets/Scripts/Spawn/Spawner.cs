using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    Dictionary<SpawnRule, bool> spawnRulesWithObectives = new();
    GameObject player;
    
    [SerializeField] List<SpawnWave> spawnWaves;
    [SerializeField] List<SpawnWave> optionalSpawnWaves;
    
    float timeElapsed = 0f;

    void Start()
    {
        player = FindFirstObjectByType<ShipMove>().gameObject;
        foreach (SpawnWave wave in spawnWaves)
        {
            wave.onSpawn += Spawn;
            wave.SpawnRulesInitialisation();
            foreach (SpawnRule sr in wave.spawnRules.spawnRules)
            {
                sr.Init();
                if (sr.waveCompletionType == WaveCompletionType.EnemiesKilled || sr.waveCompletionType == WaveCompletionType.Time)
                {
                    spawnRulesWithObectives.Add(sr, false);
                }
            }
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
        GameObject prefab = GetRandomPrefab(sr.prefabsToSpawn);
        
        float angle = Random.Range(0f, 360f);
        float distanceFromPlayer  = sr.distanceFromPlayerToSpawn;
        Vector3 center = new Vector3(player.transform.position.x + distanceFromPlayer * Mathf.Cos(angle * Mathf.Deg2Rad), 0, player.transform.position.z + distanceFromPlayer * Mathf.Sin(angle * Mathf.Deg2Rad));
        
        float objectSize = prefab.GetComponent<Renderer>() != null ? prefab.GetComponent<Renderer>()?.bounds.size.magnitude ?? 1f : prefab.GetComponent<Collider>().bounds.size.magnitude;
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
            
            if (spawnedObject.TryGetComponent(out ISpawnable spawnable))
            {
                spawnable.OnSpawn();
            }

            if (spawnedObject.TryGetComponent(out IDamageable damage) && sr.waveCompletionType == WaveCompletionType.EnemiesKilled)
            {
                damage.OnDestroy += () => ObjectDestroyed(sr);
            }
        }
    }

    void ObjectDestroyed(SpawnRule sr)
    {
        sr.UpdateCondition(1f);
        if (sr.GetKillOrTimeRemaining() <= 0 && spawnRulesWithObectives.ContainsKey(sr) && spawnRulesWithObectives[sr] == false)
        {
            Debug.Log($"Wave completed by kills.");
            spawnRulesWithObectives[sr] = true;
            CheckAllRulesCompleted();
        }
    }
    
    void Update()
    {
        TimeElapsing();
    }

    void TimeElapsing()
    {
        // Check for waves with time-based completion
        foreach (SpawnWave wave in spawnWaves)
        {
            foreach (SpawnRule sr in wave.spawnRules.spawnRules)
            {
                if (sr.waveCompletionType == WaveCompletionType.Time)
                {
                    sr.UpdateCondition(Time.deltaTime);
                    
                    if (sr.GetKillOrTimeRemaining() <= 0 && spawnRulesWithObectives.ContainsKey(sr) && spawnRulesWithObectives[sr] == false)
                    {
                        Debug.Log($"Wave completed by time.");
                        spawnRulesWithObectives[sr] = true;
                        CheckAllRulesCompleted();
                    }
                }
            }
        }
    }

    
    void CheckAllRulesCompleted()
    {
        bool allCompleted = true;
        foreach (var kvp in spawnRulesWithObectives)
        {
            if (!kvp.Value)
            {
                allCompleted = false;
                break;
            }
        }
        
        if (allCompleted)
        {
            Debug.Log("All spawn rules completed.");
            foreach (SpawnWave wave in spawnWaves)
            {
                foreach (SpawnRule sr in wave.spawnRules.spawnRules)
                {
                    wave.StopSpawnRoutine(sr);
                }
                wave.Disable();
            }
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
    Dictionary<SpawnRule, SpawnRoutineContainer> ruleToContainer = new();
    Action<SpawnRule> spawnHandler;
    
    public Action<SpawnRule> onSpawn;
    public SpawnRulesSO spawnRules;
    
    List<SpawnRoutineContainer> spawnRoutineContainers;

    public void SpawnRulesInitialisation()
    {
        spawnRoutineContainers = new List<SpawnRoutineContainer>();
        spawnHandler = OnSpawnTriggered;
        
        foreach (SpawnRule sr in spawnRules.spawnRules)
        {
            SpawnRoutineContainer spawnRoutineContainer = new GameObject("SpawnRoutineContainer").AddComponent<SpawnRoutineContainer>();
            spawnRoutineContainer.onSpawnEvent += spawnHandler;
            spawnRoutineContainer.StartSpawnRoutine(sr);
            spawnRoutineContainers.Add(spawnRoutineContainer);
            ruleToContainer[sr] = spawnRoutineContainer;
        }
    }
    
    public void StopSpawnRoutine(SpawnRule spawnRule)
    {
        if (ruleToContainer.TryGetValue(spawnRule, out SpawnRoutineContainer src))
        {
            src.StopSpawnRoutine();
        }
    }
    
    void OnSpawnTriggered(SpawnRule spawnRule)
    {
        onSpawn?.Invoke(spawnRule);
    }
    
    public void Disable()
    {
        foreach (SpawnRoutineContainer src in spawnRoutineContainers)
        {
            if (src != null)
            {
                src.onSpawnEvent -= spawnHandler;
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
    
    public void StopSpawnRoutine()
    {
        CancelInvoke(nameof(SpawnCallback));
    }
    
    void SpawnCallback()
    {
        onSpawnEvent?.Invoke(currentRule);
    }
}