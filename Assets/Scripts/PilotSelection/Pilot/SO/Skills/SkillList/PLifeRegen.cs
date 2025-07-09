using UnityEngine;
[CreateAssetMenu(fileName = "LifeRegen", menuName = "ScriptableObjects/PilotSkills/LifeRegen")]
public class PLifeRegen : Skill
{
    public int RegenEffect;
    float timer;
    public override void Apply(Ship ship, Pilot pilot)
    {
        timer += Time.deltaTime;
        if (timer >= 1f) 
        {
            if (ship.shipMove.health <= 0 || ship.shipMove.health == ship.MaxLife) return;
            ship.shipMove.health += RegenEffect;
            if (ship.shipMove.health > ship.MaxLife) ship.shipMove.health = ship.MaxLife;
            timer = 0f;
        }
    }

    public override void Remove(Ship ship, Pilot pilot)
    {
    }
}
