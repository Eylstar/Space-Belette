using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipShoot : MonoBehaviour
{
    Camera cam;
    
    InputAction rotation;
    InputAction shoot;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPoint;
    
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float shootRate = 0.1f;
    
    Quaternion initialRotation;
    Vector2 lookDirection;
    

    void Start()
    {
        cam = Camera.main;
        
        rotation = InputSystem.actions.FindAction("Aim");
        shoot = InputSystem.actions.FindAction("Shoot");
        
        rotation.performed += GetLookDirection;
        shoot.started += _ => InvokeRepeating(nameof(Shoot), 0, shootRate);
        shoot.canceled += _ => CancelInvoke(nameof(Shoot));
    }
    
    
    void GetLookDirection(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard or Mouse && Mouse.current != null)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                lookDirection = new Vector2(point.x - transform.position.x, point.z - transform.position.z);
            }
        }
        else
        {
            lookDirection = ctx.ReadValue<Vector2>();
        }
    }

    void Update()
    {
        ProcessRotation();
    }
    
    void ProcessRotation()
    {
        if (lookDirection == Vector2.zero) return;
        float angle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x, angle, initialRotation.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 25);
    }
    
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, transform.rotation * Quaternion.Euler(90, 0, 0));
        bullet.GetComponent<Projectile>().SetSpeed(bulletSpeed);
    }
    
    void OnDisable()
    {
        rotation.performed -= GetLookDirection; 
    }
}
