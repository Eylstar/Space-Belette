using UnityEngine;

public interface IPilotSkillEffect
{
    void Apply(ShipManager ship, Pilot pilot);
    void Remove(ShipManager ship, Pilot pilot);
}
