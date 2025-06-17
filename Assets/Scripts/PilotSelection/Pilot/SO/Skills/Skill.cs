using UnityEngine;

public abstract class Skill : ScriptableObject, IPilotSkillEffect
{
    public abstract void Apply(ShipManager ship, Pilot pilot);
    public abstract void Remove(ShipManager ship, Pilot pilot);
}
