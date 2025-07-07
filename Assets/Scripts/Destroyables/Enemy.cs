using System;
using UnityEngine;

public class Enemy : Destroyable, ISpawnable
{
    protected EnemiesManager enemiesManager;

    [SerializeField] protected bool canShoot;
    
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float moveSpeed;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    
    [SerializeField] protected float distanceToPlayerToShoot;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float fireRate;
    protected float timer = 0f;
    
    protected GameObject playerShip;
    protected Rigidbody rb;
    
    
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
        if (canShoot)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerShip.transform.position);
            if (distanceToPlayer <= distanceToPlayerToShoot && timer >= fireRate)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    protected virtual void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation * Quaternion.Euler(90, 0, 0));
        bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed + rb.linearVelocity.magnitude);
    }
    
    protected override void Die()
    {
        enemiesManager?.RemoveEnemy(this);
        CamShake.instance?.ShakeSmallEnemyKill();
        base.Die();
    }
}