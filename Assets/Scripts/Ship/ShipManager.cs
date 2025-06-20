using AYellowpaper.SerializedCollections;
using System;
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
    GameObject mainPilotPrefab;

    public static event Action PlayerDeath;

    private void OnEnable()
    {
        ShipColliderSetup.OnShipEnter += ShowPilot;
        ShipColliderSetup.OnShipExit += HidePilot;
    }
    private void OnDisable()
    {
        ShipColliderSetup.OnShipEnter -= ShowPilot;
        ShipColliderSetup.OnShipExit -= HidePilot;
    }
    private void Start()
    {
        shipShoot = FindFirstObjectByType<ShipShoot>();
        shipMove = FindFirstObjectByType<ShipMove>();
    }

    public void SpawnPilot()
    {
       mainPilotPrefab = Instantiate(MainPilot.pilotPrefab, shipSpawners[ShipSpawner.Cockpit].position, shipSpawners[ShipSpawner.Cockpit].rotation, transform);
    }
    void HidePilot() 
    {
        if (mainPilotPrefab != null)
        {
            mainPilotPrefab.SetActive(false);
            shipMove.OnShipExit();
        }
    }
    void ShowPilot()
    {
        if (mainPilotPrefab != null)
        {
            mainPilotPrefab.transform.position = shipSpawners[ShipSpawner.Cockpit].position;
            mainPilotPrefab.SetActive(true);
        }
    }
    private void Update()
    {
        if(MainPilot.ActiveSkill.Effect!= null) MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
        if(MainPilot.PassiveSkill.Effect!= null) MainPilot.PassiveSkill.Effect.Apply(this, MainPilot);
        if (CurrentLife <= 0)
        {
            PlayerDeath?.Invoke();
            shipShoot.enabled = false;
            shipMove.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
