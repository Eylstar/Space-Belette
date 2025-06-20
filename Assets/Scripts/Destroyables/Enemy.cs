using System;
using UnityEngine;

public class Enemy : Destroyable, ISpawnable
{
    protected EnemiesManager enemiesManager;
    
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float moveSpeed;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    
    [SerializeField] float distanceToPlayerToShoot;
    [SerializeField] float bulletSpeed;
    [SerializeField] float fireRate;
    float timer = 0f;
    
    protected GameObject playerShip;
    protected Rigidbody rb;
    
    [SerializeField] GameObject explosionPrefab;
    
    
    protected override void Start()
    {
        base.Start();
        enemiesManager = FindFirstObjectByType<EnemiesManager>(FindObjectsInactive.Include);
        playerShip = enemiesManager.GetPlayerReference();
        rb = GetComponent<Rigidbody>();
    }
    
    public void OnSpawn()
    {
        enemiesManager?.AddEnemy(this);
        Debug.Log("Added to enemy Manager");
    }

    protected void Update()
    {
        timer += Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, playerShip.transform.position);
        if (distanceToPlayer <= distanceToPlayerToShoot && timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    protected virtual void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation * Quaternion.Euler(90, 0, 0));
        bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed + rb.linearVelocity.magnitude);
    }
    
    protected override void Die()
    {
        if (explosionPrefab != null)
        {
            GameObject g = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            g.transform.localScale = Vector3.one * 4f;
        }
        enemiesManager?.RemoveEnemy(this);
        CamShake.instance?.ShakeSmallEnemyKill();
        base.Die();
    }
}