using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public enum ShipSpawner 
    {
        Cockpit,
        Cockpit2,
        CrewCabin,
    }
    public List<Bloc> ShootBlocs = new();
    public List<Bloc> ShipBlocs = new();
    public int MaxLife;
    public int CurrentLife;
    [SerializedDictionary("Ship Spawner", "Transform")]
    public SerializedDictionary<ShipSpawner, Transform> shipSpawners = new();
    public Pilot MainPilot;
    public Pilot SecondaryPilot;

    public void SpawnPilot()
    {
        Instantiate(MainPilot.pilotPrefab, shipSpawners[ShipSpawner.Cockpit].position, shipSpawners[ShipSpawner.Cockpit].rotation, transform);
    }
    private void Update()
    {
        MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
    }
}
