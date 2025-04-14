using UnityEngine;

public class Playershipmove : MonoBehaviour
{
    [Header("Vitesse de déplacement et de rotation")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;

    private Rigidbody rb;
    private Vector3 inputDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Sécurité : le Rigidbody doit être non-kinematic pour utiliser les forces physiques
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Récupération des inputs joueurs (ZQSD ou flèches)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(0f, horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        // Déplacement sur l'axe vertical
        Vector3 move = transform.forward * inputDirection.z * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Rotation sur l'axe horizontal
        if (Mathf.Abs(inputDirection.y) > 0.01f)
        {
            float rotationAmount = inputDirection.y * rotationSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
}
