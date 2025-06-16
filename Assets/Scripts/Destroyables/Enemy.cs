using System;
using UnityEngine;

public class Enemy : Destroyable, ISpawnable
{
    protected EnemiesManager enemiesManager;
    
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float moveSpeed;
    
    protected override void Start()
    {
        base.Start();
        enemiesManager = FindFirstObjectByType<EnemiesManager>(FindObjectsInactive.Include);
    }
    
    public void OnSpawn()
    {
        enemiesManager?.AddEnemy(this);
        Debug.Log("Added to enemy Manager");
    }
}