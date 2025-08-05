using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipGameplayManager : Destroyable
{
    [SerializeField] List<MonoBehaviour> skills = new();
    public int experience = 0;
    
    public static event Action PlayerDeath;
    
    protected override void Die()
        {
            PlayerDeath?.Invoke();
            gameObject.SetActive(false);
        }
}
