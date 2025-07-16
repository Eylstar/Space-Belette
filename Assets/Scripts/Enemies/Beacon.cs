using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : Destroyable, ISpawnable
{
    [Space(20), Header("Beacon Settings")]
    [SerializeField, Range(0,25)] int areaRadius = 10;
    [SerializeField] ParticleSystem halo;
    
    EnemiesManager enemiesManager;
    float maxDistanceToProtect;
    
    protected override void Start()
    {
        base.Start();
        enemiesManager = FindFirstObjectByType<EnemiesManager>(FindObjectsInactive.Include);
        OnSpawn();
        ParticleSystem.MainModule main = halo.main;
        main.startSize = areaRadius;
        maxDistanceToProtect = 7.8f * areaRadius - 4.9f;
    }
    
    public void OnSpawn()
    {
        enemiesManager?.AddBeacon(this);
    }

    protected override void Die()
    {
        enemiesManager?.RemoveBeacon(this);
        base.Die();
    }
    
    public float DistanceToProtect => maxDistanceToProtect;
}
