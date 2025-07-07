using UnityEngine;

public abstract class Skill : ScriptableObject, IPilotSkillEffect
{
    public abstract void Apply(Ship ship, Pilot pilot);
    public abstract void Remove(Ship ship, Pilot pilot);
}
