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
    //public List<Bloc> ShootBlocs = new();
    //public List<Bloc> ShipBlocs = new();
    public int MaxLife;
    public int CurrentLife;
    public Pilot MainPilot;
    public Pilot SecondaryPilot;
    public ShipShoot shipShoot;
    public ShipMove shipMove;

    public static event Action PlayerDeath;

    private void Start()
    {
        //shipShoot = FindFirstObjectByType<ShipShoot>();
        shipMove = FindFirstObjectByType<ShipMove>();
    }

    //private void Update()
    //{
    //    if (MainPilot.ActiveSkill.Effect != null) MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
    //    if (MainPilot.PassiveSkill.Effect != null) MainPilot.PassiveSkill.Effect.Apply(this, MainPilot);
    //    if (CurrentLife <= 0)
    //    {
    //        PlayerDeath?.Invoke();
    //        shipShoot.enabled = false;
    //        shipMove.enabled = false;
    //        gameObject.SetActive(false);
    //    }
    //}
}
