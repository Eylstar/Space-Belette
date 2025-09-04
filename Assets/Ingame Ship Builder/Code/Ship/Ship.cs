
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ties all the primary ship components together
/// </summary>

public class Ship : MonoBehaviour
{
    #region From Assets/IngameShip

    // Editor assignable properties
    [Tooltip("The model info holder which contains all data for this ship model")]
    public ModelInfo ShipModelInfo;
    [Tooltip("Is this ship currently piloted by the player")]
    public bool isPlayerShip = false;

    public static Ship PlayerShip;

    private void Awake()
    {
        ShipGameplayManager = FindFirstObjectByType<ShipGameplayManager>();
        //shipMove = FindFirstObjectByType<ShipMove>();
        shipShoot = FindFirstObjectByType<ShipShoot>();

    }

    private void Start()
    {
        if (isPlayerShip)
            PlayerShip = this;
        SetShipStats();
    }

    #endregion
    #region SpaceBelette

    public static Pilot MainPilot;
    public static Pilot SecondaryPilot;
    public int MaxLife = 1;
    public int lifeRegen = 0;
    public List<Transform> ShootPoints = new();
    public List<ShipProp> ShipProps;
    public List<ShipUpgradeSo> ShipWeapons = new();
    public ShipGameplayManager ShipGameplayManager;
    public ShipMove shipMove;
    public ShipShoot shipShoot;
    public int ShipCost;

    public float clockTime = 1;
    public float timer = 0;
    private void OnEnable()
    {
        PopUpUpgrade.UpgradeSelected += OnUpgradeSelected;
    }
    private void OnDisable()
    {
        PopUpUpgrade.UpgradeSelected -= OnUpgradeSelected;
    }

    private void OnUpgradeSelected(ShipUpgradeSo so)
    {
        if (so.Type == ShipUpgradeSo.UpgradeType.Weapon && so.WeaponUpgrade != null)
        {
            ShipWeapons.Add(so);
        }
        else
        {
            ApplyBasicUpgrade(so);
        }
    }
    private void ApplyBasicUpgrade(ShipUpgradeSo upgrade)
    {
        switch (upgrade.Type)
        {
            case ShipUpgradeSo.UpgradeType.MaxLife:
                Ship.PlayerShip.MaxLife += Mathf.RoundToInt(upgrade.EffectValue);
                Ship.PlayerShip.ShipGameplayManager.SetLife(Ship.PlayerShip.MaxLife);
                break;
            case ShipUpgradeSo.UpgradeType.LifeRegen:
                Ship.PlayerShip.lifeRegen += Mathf.RoundToInt(upgrade.EffectValue);
                break;
            case ShipUpgradeSo.UpgradeType.Damage:
                Ship.PlayerShip.shipShoot.SetBulletDmg(Ship.PlayerShip.shipShoot.GetBulletDmg() + (int)upgrade.EffectValue );
                break;
            case ShipUpgradeSo.UpgradeType.Speed:
                Ship.PlayerShip.shipMove.SpeedModificator(upgrade.EffectValue);
                break;
        }
    }
    void SetShipStats() 
    {
        ShipProps = new();
        int engineCount = 1;
        var props = GetComponentsInChildren<ShipProp>();
        int newDmg = shipShoot.GetBulletDmg();
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
                    if (prop.BonusDamage > 0)
                    {
                        newDmg += prop.BonusDamage;
                    }
                    break;
            }
        }
        //Debug.Log($"Pilot Name : {MainPilot.pilotName}");
        //shipMove.SpeedModificator(engineCount);
        ShipGameplayManager.SetLife(MaxLife);
        MainPilot.pilotExperience = 0;
    }

    private void Update()
    {
        if (MainPilot != null)
        {
            if (MainPilot.ActiveSkill.Effect != null) MainPilot.ActiveSkill.Effect.Apply(this, MainPilot);
            //if (MainPilot.PassiveSkill.Effect != null) MainPilot.PassiveSkill.Effect.Apply(this, MainPilot);
        }
        if (ShipWeapons.Count != 0)
        {
            foreach(ShipUpgradeSo wpn in ShipWeapons) 
            {
                if (wpn.WeaponUpgrade != null)
                {
                    wpn.Apply(this);
                }
            }
        }
        if (timer <= 0f)
        {
            timer = clockTime;
            if (ShipGameplayManager.health <= MaxLife)
            {
                ShipGameplayManager.health += lifeRegen;
                if (ShipGameplayManager.health > MaxLife) ShipGameplayManager.health = MaxLife;
                ShipGameplayManager.OnLifeChanged(ShipGameplayManager.health);
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    #endregion
}
