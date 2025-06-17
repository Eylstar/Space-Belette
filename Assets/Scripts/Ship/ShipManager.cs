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
    public ShipShoot shipShoot;
    public ShipMove shipMove;
    private void Start()
    {
        shipShoot = FindFirstObjectByType<ShipShoot>();
        shipMove = FindFirstObjectByType<ShipMove>();
    }

    public void SpawnPilot()
    {
        Instantiate(MainPilot.pilotPrefab, shipSpawners[ShipSpawner.Cockpit].position, shipSpawners[ShipSpawner.Cockpit].rotation, transform);
    }
    private void Update()
    {
        if(MainPilot.ActiveSkill.Effect!= null) MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
        if(MainPilot.PassiveSkill.Effect!= null) MainPilot.PassiveSkill.Effect.Apply(this, MainPilot);
    }
}
