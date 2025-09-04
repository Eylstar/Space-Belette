using UnityEngine;

public class ThunderSphere : Destroyable
{
    [Header("Param�tres d'orbite")]
    [SerializeField] Transform target;          // Cible autour de laquelle orbiter
    [SerializeField] float orbitRadius = 15f;   // Rayon de l�orbite
    [SerializeField] float angularSpeed = 90f;  // Vitesse angulaire en degres/s
    [SerializeField] float heightOffset = 0f;   // Decalage vertical (axe Y)
    [SerializeField] bool faceOutward = true;   // Orienter la sph�re vers l�ext�rieur
    [SerializeField] bool faceTangent = false;  // Orienter selon la tangente du mouvement

    [Header("Dur�e de vie")]
    [SerializeField] float lifeTime = 10f;      // 0 ou moins = infini

    float currentAngleDeg;                      // Angle courant en degr�s

    protected override void Start()
    {
        base.Start();

        // Planifie la destruction si une dur�e > 0 est d�finie
        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);

        // Cible par d�faut : le vaisseau du joueur s�il existe
        if (target == null && Ship.PlayerShip != null)
            target = Ship.PlayerShip.transform;

        // Initialise l�angle selon la position actuelle si d�j� plac�e pr�s de la cible
        if (target != null)
        {
            Vector3 delta = transform.position - target.position;
            delta.y = 0f;
            if (delta.sqrMagnitude > 0.0001f)
            {
                // Angle dans le plan XZ (Y up)
                currentAngleDeg = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
                orbitRadius = delta.magnitude; // Ajuste le rayon si d�j� positionn�
            }
            else
            {
                // Place sur le cercle si la position est confondue avec la cible
                currentAngleDeg = 0f;
                transform.position = target.position + new Vector3(orbitRadius, heightOffset, 0f);
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Avance l�angle
        currentAngleDeg += angularSpeed * Time.deltaTime;

        // Calcule la position sur le cercle dans le plan XZ (Y up)
        float rad = currentAngleDeg * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad) * orbitRadius, 0f, Mathf.Sin(rad) * orbitRadius);
        Vector3 center = target.position;
        transform.position = new Vector3(center.x + offset.x, center.y + heightOffset, center.z + offset.z);

        // Orientation optionnelle
        if (faceTangent)
        {
            // Direction tangente (rotation +90� dans le plan)
            Vector3 tangent = new Vector3(-Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            if (tangent.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
        }
        else if (faceOutward)
        {
            Vector3 outward = (transform.position - new Vector3(center.x, transform.position.y, center.z));
            if (outward.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(outward.normalized, Vector3.up);
        }
    }

    // API publique de configuration
    public void SetTarget(Transform t) => target = t;
    public void SetOrbitRadius(float r) => orbitRadius = Mathf.Max(0f, r);
    public void SetAngularSpeed(float degPerSec) => angularSpeed = degPerSec;
    public void SetHeightOffset(float y) => heightOffset = y;
    public void SetFaceOutward(bool value) => faceOutward = value;
    public void SetFaceTangent(bool value) => faceTangent = value;
    public void SetLifeTime(int seconds) => lifeTime = seconds;

    // Important: pour contr�ler la destruction automatique via GraviSphereSO
    public void SetLifeTime(float seconds) => lifeTime = seconds;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Vector3 center = target.position + Vector3.up * heightOffset;
        // Cercle approxim�
        const int seg = 64;
        Vector3 prev = center + new Vector3(orbitRadius, 0f, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = (i / (float)seg) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(a) * orbitRadius, 0f, Mathf.Sin(a) * orbitRadius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif
}
