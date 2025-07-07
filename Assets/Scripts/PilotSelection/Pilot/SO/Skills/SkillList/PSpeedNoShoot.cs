using UnityEngine;

public class PSpeedNoShoot : Skill
{
    // Variables
    [Header("Base value is 10 and will have no effect")]
    [Header("20 will double speedship")]
    [Header("0 will stuck the ship")]
    public float speedMultiplier;
    float startValue = 10f;
    public override void Apply(Ship ship, Pilot pilot)
    {
        if(!ship.shipShoot.IsShooting)
        {
            ship.shipMove.SpeedModificator(speedMultiplier);
        }
        else 
        { 
            ship.shipMove.SpeedModificator(startValue);
        }
    }

    public override void Remove(Ship ship, Pilot pilot)
    {
        // passive so no need
    }
}