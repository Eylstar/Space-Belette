using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Pilot", menuName = "ScriptableObjects/Pilot", order = 1)]
public class Pilot : ScriptableObject 
{
    public string pilotName;
    public int pilotLevel;
    public float pilotExperience;
    public Sprite pilotImage;
    public GameObject pilotPrefab;
    public bool IsUnlocked;
    public PilotActiveSkill ActiveSkill;
    public PilotPassiveSkill PassiveSkill;
    int xpNeeded = 1000;

    public event Action UpgradeShip;
    public event Action UpgradeWeapon;

    public void LevelUp() 
    {
        pilotLevel++;
        Debug.Log($"Pilot {pilotName} leveled up to level {pilotLevel}!"); 
        if (pilotLevel % 5 == 0)
        {
            UpgradeWeapon?.Invoke();
        }
        else 
        {
            UpgradeShip?.Invoke();
        }
    }
    public void AddExperience(float experience)
    {
        pilotExperience += experience;

        if (pilotExperience >= xpNeeded)
        {
            pilotExperience -= xpNeeded;
            LevelUp();

            if (pilotLevel <= 100) xpNeeded = Mathf.CeilToInt(xpNeeded * 1.1f);
            
            if (pilotExperience >= xpNeeded) AddExperience(0);
        }
    }
    public void ResetExperience()
    {
        pilotExperience = 0;
        pilotLevel = 1;
        xpNeeded = 1000;
    }
}
