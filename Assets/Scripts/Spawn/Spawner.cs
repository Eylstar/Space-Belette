using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    Dictionary<SpawnRule, bool> spawnRulesWithObectives = new();
    Dictionary<SpawnRule, int> spawnedObjectsCount = new();
    GameObject player;
    
    [SerializeField] List<SpawnWave> spawnWaves;
    [SerializeField] List<SpawnWave> optionalSpawnWaves;

    public static event Action EndMission;

    float timeElapsed = 0f;

    private void OnEnable()
    {
        /*spawnWaves = new();
        optionalSpawnWaves = new();

        foreach (SpawnRulesSO wave in MissionManager.CurrentMission.MainWaves)
        {
            spawnWaves.Add(new SpawnWave { spawnRules = wave });
        }
        foreach (SpawnRulesSO wave in MissionManager.CurrentMission.OptionalWaves)
        {
            optionalSpawnWaves.Add(new SpawnWave { spawnRules = wave });
        }*/
    }

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
        if (spawnedObjectsCount.TryGetValue(sr, out int count) && count >= sr.maxSpawnCount) return;
        
        int c = sr.spawnCount + Mathf.RoundToInt(sr.spawnCount * sr.spawnDeltaPercentage * Random.Range(-1f, 1f));
        GameObject prefab = GetRandomPrefab(sr.prefabsToSpawn);

        float angle = Random.Range(0f, 360f);
        float distanceFromPlayer  = sr.distanceFromPlayerToSpawn;
        Vector3 center = new Vector3(player.transform.position.x + distanceFromPlayer * Mathf.Cos(angle * Mathf.Deg2Rad), 0, player.transform.position.z + distanceFromPlayer * Mathf.Sin(angle * Mathf.Deg2Rad));
        
        float objectSize = prefab.TryGetComponent<Renderer>(out var r) ? r.bounds.size.magnitude : prefab.TryGetComponent<Collider>(out var col) ? col.bounds.size.magnitude : 1f;
        float spacing = objectSize * sr.groupDispersion;
        
        float radius = c > 1 ? spacing / (2f * Mathf.Sin(Mathf.PI / c)) : 0f;
        
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
            
            if (spawnedObjectsCount.ContainsKey(sr))
                spawnedObjectsCount[sr]++;
            else
                spawnedObjectsCount[sr] = 1;
            
            spawnedObject.transform.parent = transform;
            
            if (spawnedObject.TryGetComponent(out ISpawnable spawnable))
            {
                spawnable.OnSpawn();
            }

            if (spawnedObject.TryGetComponent(out IDamageable damage))
            {
                damage.OnDestroyAction += () => ObjectDestroyed(sr);
                if (sr.waveCompletionType == WaveCompletionType.EnemiesKilled)
                {
                    damage.OnDestroyAction += () => EnemyKilled(sr);
                }
            }
        }
    }

    void ObjectDestroyed(SpawnRule sr)
    {
        if (!spawnedObjectsCount.ContainsKey(sr)) return;
        if (spawnedObjectsCount[sr] > 0)
        {
            spawnedObjectsCount[sr]--;
        }
    }
    
    void EnemyKilled(SpawnRule sr)
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
            EndMission?.Invoke();
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