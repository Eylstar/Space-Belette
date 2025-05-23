using UnityEngine;
[CreateAssetMenu(fileName = "LifeRegen", menuName = "ScriptableObjects/PilotSkills/LifeRegen")]
public class LifeRegen : Skill
{
    public int RegenEffect;
    float timer;
    public override void Apply(ShipManager ship, Pilot pilot)
    {
        timer += Time.deltaTime;
        if (timer >= 1f) 
        {
            if (ship.CurrentLife <= 0 || ship.CurrentLife == ship.MaxLife) return;
            ship.CurrentLife += RegenEffect;
            if (ship.CurrentLife > ship.MaxLife) ship.CurrentLife = ship.MaxLife;
            timer = 0f;
        }
    }

    public override void Remove(ShipManager ship, Pilot pilot)
    {
    }
}
