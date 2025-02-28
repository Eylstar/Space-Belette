using UnityEngine;

[CreateAssetMenu(fileName = "AsteroidStatsSO", menuName = "Scriptable Objects/AsteroidStatsSO")]
public class AsteroidStatsSO : ScriptableObject
{
    [Header("General Settings")]
    public int ScoreValue;
    public int MaxHealth;
    [Space(10), Header("Asteroid Settings")]
    public int AsteroidDivisions;
    public int ChildSpawnCount;
    public float SpeedIncrement;
    public float SizeDecrement;
    public float AsteroidSize;
    public float AsteroidSpeed;
    public GameObject AsteroidPrefab;
}

public struct AsteroidStatsStruct
{
    public readonly int ScoreValue;
    public readonly int MaxHealth;
    public readonly int AsteroidDivisions;
    public readonly int ChildSpawnCount;
    public readonly float SpeedIncrement;
    public readonly float SizeDecrement;
    public readonly float AsteroidSize;
    public readonly GameObject AsteroidPrefab;
    public float AsteroidSpeed;
    
    public AsteroidStatsStruct(AsteroidStatsSO stats)
    {
        ScoreValue = stats.ScoreValue;
        MaxHealth = stats.MaxHealth;
        AsteroidDivisions = stats.AsteroidDivisions;
        ChildSpawnCount = stats.ChildSpawnCount;
        SpeedIncrement = stats.SpeedIncrement;
        SizeDecrement = stats.SizeDecrement;
        AsteroidSize = stats.AsteroidSize;
        AsteroidSpeed = stats.AsteroidSpeed;
        AsteroidPrefab = stats.AsteroidPrefab;
    }
}