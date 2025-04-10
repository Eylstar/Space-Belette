using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    GameObject player;
    List<Enemy> enemies = new();
    
    GameObject GetPlayerReference() => player;
    Transform GetPlayerTransform() => player.transform;
    
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
