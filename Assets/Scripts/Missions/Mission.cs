using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Mission", menuName = "ScriptableObjects/Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string missionName;
    [TextArea] public string description;
    public int missionID;
    public bool isCompleted = false;
    public int rewardCredits;
    public int rewardExperience;
    public List<GameObject> rewardItems;
    public bool isActive = false;
    public List<SpawnRulesSO> MainWaves;
    public List<SpawnRulesSO> OptionalWaves;
}
