using UnityEngine;
public class PilotSkills : ScriptableObject
{
    public string SkillName;
    [TextArea] public string Description;
    public GameObject EffectPrefab;
}