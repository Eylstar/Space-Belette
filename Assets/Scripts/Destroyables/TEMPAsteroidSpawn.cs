using System.Collections.Generic;
using UnityEngine;

public class TEMPAsteroidSpawn : MonoBehaviour
{
    public List<Transform> spawnPoint;
    public AsteroidStatsSO stats;
    AsteroidStatsStruct statsStruct;
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroid), 0, 2);
        statsStruct = new AsteroidStatsStruct(stats);
    }
    
    void SpawnAsteroid()
    {
        GameObject go = Instantiate(stats.AsteroidPrefab, spawnPoint[Random.Range(0, spawnPoint.Capacity-1)].position, Quaternion.Euler(0, Random.Range(0, 360), 0));
        go.transform.localScale = Vector3.one * statsStruct.AsteroidSize;
        go.GetComponent<Asteroid>().SetupAsteroid(statsStruct, 0);
    }
}
