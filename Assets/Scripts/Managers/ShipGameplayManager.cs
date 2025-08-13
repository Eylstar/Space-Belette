using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipGameplayManager : Destroyable
{
    [SerializeField] Collider xpTriggerCollider;
    [SerializeField] List<MonoBehaviour> skills = new();
    public float experience = 0;
    
    public static event Action PlayerDeath;
    
    protected override void Die()
    {
            PlayerDeath?.Invoke();
            gameObject.SetActive(false);
    }
    
    public void GainExperience(float amount)
    {
        experience += amount;
    }
}
