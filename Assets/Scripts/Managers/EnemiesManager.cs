using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    List<Enemy> enemies = new();
    List<Beacon> beacons = new();
    
    Coroutine beaconsCoroutine;
    float beaconCheckInterval = 0.5f;
    
    public GameObject GetPlayerReference() => player;
    public Transform GetPlayerTransform() => player.transform;

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void AddBeacon(Beacon beacon) => beacons.Add(beacon);
    public void RemoveEnemy(Enemy enemy) => enemies.Remove(enemy);
    
    public void RemoveBeacon(Beacon beacon)
    {
        beacons.Remove(beacon);
        if (beacons.Count == 0 && beaconsCoroutine != null)
        {
            StopCoroutine(beaconsCoroutine);
            beaconsCoroutine = null;
        }
    }
    
    public List<Enemy> GetInRangeEnemies(Vector3 position, float range)
    {
        return enemies.Where(enemy => Vector3.Distance(position, enemy.transform.position) <= range).ToList();
    }
    
    public List<Enemy> GetAllEnemies()
    {
        return enemies;
    }

    void Start()
    {
        beaconsCoroutine = StartCoroutine(BeaconsCheck());
    }
    
    IEnumerator BeaconsCheck()
    {
        Debug.Log("Beacons check started");
        while (true)
        {
            if (beacons.Count == 0)
            {
                yield return new WaitForSeconds(beaconCheckInterval);
                continue;
            }
            
            HashSet<Enemy> inRangeSet = new();
            foreach (Beacon b in beacons)
            {
                inRangeSet.UnionWith(GetInRangeEnemies(b.transform.position, b.DistanceToProtect));
                Debug.Log($"Beacon at {b.transform.position} has {inRangeSet.Count} enemies in range.");
            }
            foreach (Enemy e in enemies)
            {
                e.canBeDamaged = !inRangeSet.Contains(e);
            }
            yield return new WaitForSeconds(beaconCheckInterval);
        }
    }
}