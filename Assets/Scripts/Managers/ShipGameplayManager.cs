using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipGameplayManager : Destroyable
{
    [SerializeField] Collider xpTriggerCollider;
    [SerializeField] Ship ship;
    [SerializeField] List<MonoBehaviour> skills = new();
    public float experience = 0;
    
    public static event Action<int> OnLifeSetup, OnLifeChange;
    
    public static event Action PlayerDeath;
    private void OnEnable()
    {
        ship = Ship.PlayerShip;
    }
    protected override void Die()
    {
        PlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
    
    public void GainExperience(float amount)
    {
        experience += amount;
        Ship.MainPilot?.AddExperience(amount);
    }
    
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        OnLifeChange?.Invoke(health);
        if (health <= 0)
        {
            Die();
        }
    }
    
    public new void SetLife(int life)
    {
        base.SetLife(life);
        OnLifeSetup?.Invoke(life);
    }
    
    public void OnLifeChanged(int newLife)
    {
        OnLifeChange?.Invoke(health);
    }
}
