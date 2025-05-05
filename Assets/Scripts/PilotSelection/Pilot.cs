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

}
