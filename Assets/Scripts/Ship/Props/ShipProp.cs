using UnityEngine;

public class ShipProp : MonoBehaviour
{
    public enum PropType
    {
        Weapon,
        Engine,
        Utility
    }
    public string PropName;
    public PropType Type;
    public Transform[] ShootPoints;
    public int BonusLife;
    public int EngineCount;
    public int BonusDamage;
    public int BonusShield;
    public int LifeRegen;
    public int Cost;
    public bool IsStartProp = false;
}
