using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraviSphereSO : NewWeapon
{
    private Coroutine weaponEffectCoroutine;
    private readonly List<GameObject> activeSpheres = new();
    private MonoBehaviour runnerRef;
    private int runningLevel = -1;

    [Header("Réglages généraux")]
    [SerializeField] private float cooldown = 12.5f; // Longueur d'un cycle complet

    [Header("Prefab de l'orbe")]
    public GameObject SpherePrefab;

    // Réglages d’orbite
    private const float OrbitPeriod = 2.5f;                // Un tour en 2.5s
    private const float AngularSpeed = 360f / OrbitPeriod; // 144 deg/s
    [SerializeField] private float radius = 15f;
    [SerializeField] private float height = 0f;

    public override void Apply(int lvl, MonoBehaviour runner)
    {
        if (runner == null || SpherePrefab == null || Ship.PlayerShip == null)
            return;

        lvl = Mathf.Clamp(lvl, 1, 5);

        // Démarrage initial
        if (weaponEffectCoroutine == null)
        {
            runnerRef = runner;
            runningLevel = lvl;
            weaponEffectCoroutine = runnerRef.StartCoroutine(WeaponEffect(lvl));
            return;
        }

        // Si le niveau a changé, on redémarre proprement
        if (lvl != runningLevel && runnerRef != null)
        {
            runnerRef.StopCoroutine(weaponEffectCoroutine);
            weaponEffectCoroutine = null;
            runningLevel = lvl;

            DestroyActiveSpheres();

            weaponEffectCoroutine = runnerRef.StartCoroutine(WeaponEffect(lvl));
        }
    }

    public override IEnumerator WeaponEffect(int lvl)
    {
        int sphereCount = Mathf.Clamp(lvl, 1, 5);

        // Règle: lifetime = cooldown * (lvl / 5f)
        float lifetime = cooldown * (lvl / 5f);

        while (true)
        {
            // Nettoyage avant nouveau cycle
            DestroyActiveSpheres();

            // Spawn réparti uniformément
            for (int i = 0; i < sphereCount; i++)
            {
                float angleDeg = (360f / sphereCount) * i;
                var go = SpawnSphereAtAngle(angleDeg);
                if (go == null) continue;

                activeSpheres.Add(go);

                // Configure l'orbe
                var ts = go.GetComponent<ThunderSphere>();
                if (ts != null)
                {
                    // Indestructible par dégâts/collisions: seule la fin de vie détruit
                    ts.canBeDamaged = false;

                    ts.SetTarget(Ship.PlayerShip.transform);
                    ts.SetOrbitRadius(radius);
                    ts.SetHeightOffset(height);
                    ts.SetAngularSpeed(AngularSpeed);
                    ts.SetFaceOutward(true);
                    ts.SetFaceTangent(false);
                    ts.SetLifeTime(lifetime);
                }

                // Durée de vie gérée ici (indépendante du prefab)
                if (lifetime > 0f)
                    Object.Destroy(go, lifetime);
            }

            // Cycle complet = cooldown
            yield return new WaitForSeconds(cooldown);
        }
    }

    private GameObject SpawnSphereAtAngle(float angleDeg)
    {
        Transform target = Ship.PlayerShip.transform;

        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius);
        Vector3 pos = target.position + new Vector3(offset.x, height, offset.z);

        return Object.Instantiate(SpherePrefab, pos, Quaternion.identity);
    }

    private void DestroyActiveSpheres()
    {
        for (int i = 0; i < activeSpheres.Count; i++)
        {
            if (activeSpheres[i] != null)
                Object.Destroy(activeSpheres[i]);
        }
        activeSpheres.Clear();
    }
}