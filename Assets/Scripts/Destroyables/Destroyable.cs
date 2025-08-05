using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Destroyable : MonoBehaviour, IDamageable, ICollidable
{
    [SerializeField] protected int hitPower = 1;
    [SerializeField] protected int xpValue = 0;
    protected int scoreValue;
    protected int maxHealth;
    public int health;
    public bool canBeDamaged = true;
    
    [SerializeField] protected CollidableType collisionType;
    [SerializeField] protected List<CollidableType> collisionFilter = new();
    
    public List<CollidableType> GetFilter() => collisionFilter;
    public CollidableType GetCollidableType() => collisionType;
    
    [SerializeField] GameObject explosionPrefab;
    
    protected virtual void Start()
    {
        health = maxHealth;
    }
    
    public void SetLife(int life)
    {
        maxHealth = life;
        health = life;
    }

    public virtual void TakeDamage(int damage)
    {
        if (!canBeDamaged)
            return;
        
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Destroyable " + gameObject.name + " destroyed" + " with " + damage + " damage");
            Die();
        }
    }

    public Action OnDestroyAction { get; set; }

    protected virtual void Die()
    {
        if (explosionPrefab != null)
        {
            GameObject g = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            g.transform.localScale = Vector3.one * 4f;
        }
        Destroy(gameObject);
        OnDestroyAction?.Invoke();
    }
    
    protected virtual void OnCollisionEnter(Collision other)
    {
        //If the other object is collidable
        if (other.gameObject.TryGetComponent(out ICollidable collidable))
        {
            //Debug.Log("Collision with " + collidable.GetCollidableType() + "  " + collidable.GetGameObject().name + " by " + gameObject.name);
            //If the other object is in the filter
            if (collidable.GetFilter().Contains(collisionType))
            {
                //If the other object is damageable
                if (collidable.GetGameObject().TryGetComponent(out IDamageable damageable))
                {
                    //Debug.Log("Deal damage to " + collidable.GetGameObject().name);
                    //Deal damage
                    damageable.TakeDamage(hitPower);
                }
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //If the other object is collidable
        if (other.gameObject.TryGetComponent(out ICollidable collidable))
        {
            Debug.Log("Trigger with " + collidable.GetCollidableType() + "  " + collidable.GetGameObject().name + " by " + gameObject.name);
            //If the other object is in the filter
            if (collidable.GetFilter().Contains(collisionType))
            {
                //If the other object is damageable
                if (collidable.GetGameObject().TryGetComponent(out IDamageable damageable))
                {
                    //Debug.Log("Deal damage to " + collidable.GetGameObject().name);
                    //Deal damage
                    damageable.TakeDamage(hitPower);
                }
            }
        }
    }
    

    public GameObject GetGameObject() => gameObject;
}
