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
            if (ship.ShipGameplayManager.health <= 0 || ship.ShipGameplayManager.health == ship.MaxLife) return;
            ship.ShipGameplayManager.health += RegenEffect;
            if (ship.ShipGameplayManager.health > ship.MaxLife) ship.ShipGameplayManager.health = ship.MaxLife;
            timer = 0f;
        }
    }

    public override void Remove(Ship ship, Pilot pilot)
    {
    }
}
