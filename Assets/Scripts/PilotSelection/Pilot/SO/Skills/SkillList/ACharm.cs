using System.Collections.Generic;
using UnityEngine;

public class ACharm : Skill
{
    private bool isActive = false;
    private float effectTimer = 0f;
    private float cooldownTimer = 0f;
    public float effectDuration = 1f;
    public float cooldownDuration = 30f;

    private List<Rigidbody> frozenBodies = new();

    public override void Apply(Ship ship, Pilot pilot)
    {
        if (!isActive && cooldownTimer <= 0f && Input.GetKeyDown(KeyCode.Space))
        {
            Activate();
            Debug.LogWarning("Skill activated: Tous les ennemis visibles sont figés !");
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

    private void Activate()
    {
        isActive = true;
        effectTimer = effectDuration;
        frozenBodies.Clear();

        List<Enemy> enemies = FindFirstObjectByType<EnemiesManager>().GetAllEnemies();
        foreach (Enemy enemy in enemies)
        {
            Renderer renderer = enemy.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.isVisible)
            {
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    //rb.gameObject.GetComponent<Shoot>().IsShooting = false;
                    frozenBodies.Add(rb);
                }
                else
                {
                    // Si pas de Rigidbody, on peut désactiver les scripts de mouvement si besoin
                    MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
                    foreach (var script in scripts)
                        script.enabled = false;
                }
            }
        }
    }

    public override void Remove(Ship ship, Pilot pilot)
    {
        if (isActive)
        {
            isActive = false;
            foreach (Rigidbody rb in frozenBodies)
            {
                rb.constraints = RigidbodyConstraints.None;
                //rb.gameObject.GetComponent<Shoot>().IsShooting = true;
            }
            frozenBodies.Clear();
        }
    }
}
