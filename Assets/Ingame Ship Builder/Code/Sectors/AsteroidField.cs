using UnityEngine;

/// <summary>
/// Asteroid field spawner to provide a background
/// Influenced by https://github.com/brihernandez/UnityCommon/blob/master/Assets/RandomAreaSpawner/Code/RandomAreaSpawner.cs
/// </summary>
public class AsteroidField : MonoBehaviour
{
    [Header("Field settings:")]
    [Tooltip("Prefab to spawn.")]
    public Transform rockPrefab;
    [Tooltip("Number of asteroids")]
    public int asteroidCount = 50;
    [Tooltip("Distance from the center of the asteroid field that asteroids will spawn")]
    public float range = 1000.0f;
    [Tooltip("Randomly rotate asteroids if true")]
    public bool randomRotation = true;
    [Tooltip("Size interval")]
    public Vector2 scaleRange = new Vector2(1.0f, 3.0f);

    [Header("Asteroid settings:")]
    [Tooltip("Apply a velocity between 0 to this value in a random direction.")]
    public float velocity = 0.0f;
    [Tooltip("Apply an angular velocity (deg/s) between 0 and this value in a random direction.")]
    public float angularVelocity = 0.0f;
    [Tooltip("Scale mass of the object based on its size.")]
    public bool scaleMassBySize = true;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < asteroidCount; i++)
            CreateAsteroid();
    }

    private void CreateAsteroid()
    {
        Vector3 spawnPos = Vector3.zero;
         
        // Create random position based on specified shape and range.
        spawnPos = Random.insideUnitSphere * range;

        // Offset position to match position of the parent gameobject.
        spawnPos += transform.position;

        // Apply a random rotation if necessary.
        Quaternion spawnRot = (randomRotation) ? Random.rotation : Quaternion.identity;

        // Create the object and set the parent to this gameobject for scene organization.
        Transform t = Instantiate(rockPrefab, spawnPos, spawnRot) as Transform;
        t.SetParent(transform);

        // Apply scaling.
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        t.localScale = Vector3.one * scale;

        // Apply rigidbody values.
        Rigidbody r = t.GetComponent<Rigidbody>();
        if (r)
        {
            if (scaleMassBySize)
                r.mass *= scale * scale * scale;

            r.AddRelativeForce(Random.insideUnitSphere * velocity, ForceMode.VelocityChange);
            r.AddRelativeTorque(Random.insideUnitSphere * angularVelocity * Mathf.Deg2Rad, ForceMode.VelocityChange);
        }
    }

}
