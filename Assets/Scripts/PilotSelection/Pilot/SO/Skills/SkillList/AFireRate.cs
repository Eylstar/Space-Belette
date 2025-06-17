using UnityEngine;

public class AFireRate : Skill
{
    public float Rate = 2f;
    private bool isActive = false;
    private float effectTimer = 0f;
    private float cooldownTimer = 0f;
    public float effectDuration = 4f;
    public float cooldownDuration = 20f;

    public override void Apply(ShipManager ship, Pilot pilot)
    {
        // Appelé à chaque frame depuis ShipManager.Update()
        if (!isActive && cooldownTimer <= 0f && Input.GetKeyDown(KeyCode.Space))
        {
            Activate(ship);
            Debug.LogWarning("Skill activated: Fire Rate increased.");
        }

        if (isActive)
        {
            effectTimer -= Time.deltaTime;
            if (effectTimer <= 0f)
            {
                Remove(ship, pilot);
                cooldownTimer = cooldownDuration;
                Debug.LogWarning("Skill effect ended, cooldown started.");
            }
        }
        else if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void Activate(ShipManager ship)
    {
        isActive = true;
        effectTimer = effectDuration;
        ship.shipShoot.factor *= Rate;
        ship.shipShoot.UpdateShootRate();
        ship.shipMove.zigzagAmplitude = 100f;
        ship.shipMove.zigzagFrequency = effectDuration;
        ship.shipMove.ZigzagActive = true;
        // Tu peux ajouter un feedback visuel ici
    }

    public override void Remove(ShipManager ship, Pilot pilot)
    {
        if (isActive)
        {
            isActive = false;
            ship.shipShoot.factor = 1;
            ship.shipShoot.UpdateShootRate();
            ship.shipMove.ZigzagActive = false;
            ship.shipMove.zigzagAmplitude = 0f;
            ship.shipMove.zigzagFrequency = 0f;
            // Tu peux ajouter un feedback visuel ici
        }
    }
}