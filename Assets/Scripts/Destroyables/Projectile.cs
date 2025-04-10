using UnityEngine;

public class Projectile : Destroyable
{
    [SerializeField] float lifeTime = 5f;
    float speed = 10f;
    
    protected override void Start()
    {
        base.Start();
        Destroy(gameObject, lifeTime);
    }

    public void SetSpeed(float s) => speed = s;
    
    void Update()
    {
        transform.position += transform.up * (Time.deltaTime * speed);
    }
}