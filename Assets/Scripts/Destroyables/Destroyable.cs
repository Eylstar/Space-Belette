using System;
using UnityEngine;

public class Destroyable : MonoBehaviour, IDamageable
{
    public Action<int> onDeath;
    
    protected int scoreValue;
    protected int maxHealth;
    int health;
    
    void Start()
    {
        health = maxHealth;
    }
    
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(Die), 0.1f);
        }
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
        onDeath?.Invoke(scoreValue);
    }
}
