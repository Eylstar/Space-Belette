using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Destroyable : MonoBehaviour, IDamageable, ICollidable
{
    [SerializeField] protected int hitPower = 1;
    [SerializeField] protected int xpValue = 0;
    protected int maxHealth;
    public int health;
    public bool canBeDamaged = true;
    
    [SerializeField] protected CollidableType collisionType;
    [SerializeField] protected List<CollidableType> collisionFilter = new();
    
    protected int xpToLoot = 0;
    [SerializeField] GameObject xpOrb;
    
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
        if (xpOrb != null && xpValue > 0)
        {
            float orbs = UnityEngine.Random.Range(1, 5);
            for (int i = 0; i < orbs; i++)
            {
                GameObject xp = Instantiate(xpOrb, transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity);
                xp.transform.localScale = Vector3.one / (orbs/2);
                xp.GetComponent<XPContainer>().SetExperience(xpValue / orbs);
            }
        }
        Destroy(gameObject);
        OnDestroyAction?.Invoke();
    }
    
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ignore" || gameObject.tag == "Ignore")
        {
            Debug.Log("Collision with Ignore object: " + other.gameObject.name);
            return; // Ignore collisions with objects tagged as "Ignore"
        }
        //If the other object is collidable
        if (other.gameObject.TryGetComponent(out ICollidable collidable))
        {
            Debug.Log("Collision with " + collidable.GetCollidableType() + "  " + collidable.GetGameObject().name + " by " + gameObject.name);
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
        if (other.gameObject.tag == "Ignore" || gameObject.tag == "Ignore")
        {
            Debug.Log("Trigger with Ignore object: " + other.gameObject.name);
            return; // Ignore collisions with objects tagged as "Ignore"
        }
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
