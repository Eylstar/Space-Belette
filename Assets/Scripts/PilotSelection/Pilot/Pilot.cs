using UnityEngine;
[CreateAssetMenu(fileName = "Pilot", menuName = "ScriptableObjects/Pilot", order = 1)]
public class Pilot : ScriptableObject 
{
    public string pilotName;
    public int pilotLevel;
    public int pilotExperience;
    public Sprite pilotImage;
    public GameObject pilotPrefab;
    public bool IsUnlocked;
    public PilotActiveSkill ActiveSkill;
    public PilotPassiveSkill PassiveSkill;
    int xpNeeded = 1000;

    public void LevelUp() 
    {
        pilotLevel++;
    }
    public void AddExperience(int experience)
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
}
