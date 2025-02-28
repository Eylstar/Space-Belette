using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] float lifeTime = 5f;
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit something");
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}