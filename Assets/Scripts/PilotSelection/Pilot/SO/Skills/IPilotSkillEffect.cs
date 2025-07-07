using UnityEngine;

public interface IPilotSkillEffect
{
    void Apply(Ship ship, Pilot pilot);
    void Remove(Ship ship, Pilot pilot);
}
