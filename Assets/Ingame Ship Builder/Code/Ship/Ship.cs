using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Ties all the primary ship components together
/// </summary>

public class Ship : MonoBehaviour
{
    #region From Assets/IngameShip

    //public bool UsingMouseInput
    //{
    //    set { playerInput.useMouseInput = value; }
    //    get { return playerInput.useMouseInput; }
    //}

    //public Vector3 Velocity
    //{
    //    get { return rbody.linearVelocity; }
    //}

    //public float Throttle
    //{
    //    get { return playerInput.throttle; }
    //}

    //public bool InSupercruise {
    //    get { return thrusters.InSupercruise; }
    //    set { thrusters.InSupercruise = value;    }
    //}

    // Editor assignable properties
    [Tooltip("The model info holder which contains all data for this ship model")]
    public ModelInfo ShipModelInfo;
    [Tooltip("Is this ship currently piloted by the player")]
    public bool isPlayerShip = false;

    public static Ship PlayerShip;

    private void Awake()
    {
        shipMove = FindFirstObjectByType<ShipMove>();
        shipShoot = FindFirstObjectByType<ShipShoot>();

    }

    private void Start()
    {
        if (isPlayerShip)
            PlayerShip = this;
        SetShipStats();
    }

    /// <summary>
    /// Turns the engine on and off.
    /// </summary>
    //public void ToggleEngine(Light engineTorch)
    //{
    //    thrusters.ToggleEngines(engineTorch);
    //}
    #endregion
    #region SpaceBelette

    public static Pilot MainPilot;
    public static Pilot SecondaryPilot;
    public int CurrentLife;
    public int MaxLife = 1;
    int lifeRegen = 0;
    public List<Transform> ShootPoints = new();
    public List<ShipProp> ShipProps;
    public ShipMove shipMove;
    public ShipShoot shipShoot;
    public static event Action PlayerDeath;
    float clockTime = 1;
    float timer = 0;
    void SetShipStats() 
    {
        ShipProps = new();
        int engineCount = 0;
        var props = GetComponentsInChildren<ShipProp>();
        foreach (var prop in props)
        {
            ShipProps.Add(prop);
        }
        foreach (ShipProp prop in ShipProps)
        {
            switch(prop.Type)
            {
                case ShipProp.PropType.Weapon:
                    foreach (var shootPoint in prop.ShootPoints)
                    {
                        if (shootPoint == null) continue;
                        Vector3 localPos = shootPoint.localPosition;
                        localPos.y = 0f;
                        shootPoint.localPosition = localPos;
                        shipShoot.shootPoints.Add(shootPoint);
                    }

                    break;
                case ShipProp.PropType.Engine:
                    MaxLife += prop.BonusLife;
                    engineCount += 1;
                    break;
                case ShipProp.PropType.Utility:
                    MaxLife += prop.BonusLife;
                    lifeRegen += prop.LifeRegen;
                    break;
            }
        }
        Debug.Log($"Engine number : {engineCount}");
        shipMove.SpeedModificator(engineCount);
        CurrentLife = MaxLife;
    }

    private void Update()
    {
        if (MainPilot.ActiveSkill.Effect != null) MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
        if (MainPilot.PassiveSkill.Effect != null) MainPilot.PassiveSkill.Effect.Apply(this, MainPilot);
        if (CurrentLife <= 0)
        {
            PlayerDeath?.Invoke();
            shipShoot.enabled = false;
            shipMove.enabled = false;
            gameObject.SetActive(false);
        }
        if (timer <= 0f)
        {
            timer = clockTime;
            if (CurrentLife < MaxLife)
            {
                CurrentLife += lifeRegen;
                if (CurrentLife > MaxLife) CurrentLife = MaxLife;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    #endregion
}
