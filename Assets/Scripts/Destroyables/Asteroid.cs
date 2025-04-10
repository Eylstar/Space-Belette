using UnityEngine;

public class Asteroid : Obstacle
{
    Rigidbody rb;
    AsteroidStatsStruct stats;
    int actualDivision;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = new AsteroidStatsStruct();
    }

    public void SetupAsteroid(AsteroidStatsStruct s, int division)
    {
        //Set the stats of the asteroid
        stats = s;
        actualDivision = division;
        scoreValue = stats.ScoreValue;
        maxHealth = stats.MaxHealth;
        LaunchAsteroid();
    }
    
    void LaunchAsteroid()
    {
        //Add force to the asteroid
        rb.AddForce(transform.forward * stats.AsteroidSpeed, ForceMode.VelocityChange);
    }

    protected override void Die()
    {
        //If the asteroid has divisions left, spawn children asteroids
        if (actualDivision < stats.AsteroidDivisions)
        {
            float angle = 0;
            //Increase the speed of the children asteroids
            stats.AsteroidSpeed *= stats.SpeedIncrement;
            for (int i = 0; i < stats.ChildSpawnCount; i++)
            {
                //Create the children asteroids
                GameObject asteroid = Instantiate(stats.AsteroidPrefab, transform.position, transform.rotation);
                asteroid.transform.Rotate(Vector3.up, angle);
                
                //Decrease the size of the children asteroids
                asteroid.transform.localScale = transform.localScale / stats.SizeDecrement;
                asteroid.transform.position += asteroid.transform.forward * stats.AsteroidSize * 1.5f;
                
                //Setup the children asteroids with the stats and division
                asteroid.GetComponent<Asteroid>().SetupAsteroid(stats, actualDivision + 1);
                
                angle += 360f / stats.ChildSpawnCount;
            }
        }
        base.Die();
    }
    
    protected override void OnCollisionEnter(Collision other)
    {
        //Check of the other object is collidable
        if (!other.gameObject.TryGetComponent(out ICollidable damageable)) return;
        
        //If asteroid collides with asteroid destroy both
        if (damageable.GetCollidableType() == collisionType)
        {
            Destroy(damageable.GetGameObject());
            Destroy(gameObject);
        }
        else
        {
            base.OnCollisionEnter(other);
        }
    }
}