using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ShipUpgrade", menuName = "ScriptableObjects/ShipUpgrade", order = 1)]
public class ShipUpgradeSo : ScriptableObject
{
    public enum UpgradeType
    {
        MaxLife,
        LifeRegen,
        Damage,
        Speed,
        Weapon
    }
    public UpgradeType Type;
    public Sprite Icon;
    public string UpgradeName;
    public bool IsUpgradable = true;
    public int Level = 1;
    public int MaxLevel = 5;
    [Header(@"/!\ Valeur brut sauf pour la speed qui est en % /!\ inutile pour weapon")]
    public float EffectValue;
    public NewWeapon WeaponUpgrade;

    public void Apply(MonoBehaviour runner) 
    {
        if (WeaponUpgrade != null)
        {
            WeaponUpgrade.Apply(Level, runner);
        }
    }
}

