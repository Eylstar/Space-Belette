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
        stats = s;
        actualDivision = division;
        scoreValue = stats.ScoreValue;
        maxHealth = stats.MaxHealth;
        LaunchAsteroid();
    }
    
    void LaunchAsteroid()
    {
        rb.AddForce(transform.forward * stats.AsteroidSpeed, ForceMode.VelocityChange);
    }

    protected override void Die()
    {
        if (actualDivision < stats.AsteroidDivisions)
        {
            float angle = 0;
            stats.AsteroidSpeed *= stats.SpeedIncrement;
            for (int i = 0; i < stats.ChildSpawnCount; i++)
            {
                GameObject asteroid = Instantiate(stats.AsteroidPrefab, transform.position, transform.rotation);
                asteroid.transform.Rotate(Vector3.up, angle);
                asteroid.transform.localScale = transform.localScale / stats.SizeDecrement;
                asteroid.transform.position += asteroid.transform.forward * stats.AsteroidSize;
                asteroid.GetComponent<Asteroid>().SetupAsteroid(stats, actualDivision + 1);
                angle += 360f / stats.ChildSpawnCount;
            }
        }
        base.Die();
    }
}
