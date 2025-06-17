using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    List<Enemy> enemies = new();
    
    public GameObject GetPlayerReference() => player;
    public Transform GetPlayerTransform() => player.transform;

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
    
    public List<Enemy> GetInRangeEnemies(Vector3 position, float range)
    {
        return enemies.Where(enemy => Vector3.Distance(position, enemy.transform.position) <= range).ToList();
    }
    
    public List<Enemy> GetAllEnemies()
    {
        return enemies;
    }
}
