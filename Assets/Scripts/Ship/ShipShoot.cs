using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ShipShoot : MonoBehaviour
{
    List<Bloc> weapons = new();
    List<Transform> shootPoints = new();
    Camera cam;
    
    InputAction rotation;
    InputAction shoot;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPoint;
    ShipManager shipManager;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float shootRate = 0.1f;
    public float factor = 1;
    Quaternion initialRotation;
    Vector2 lookDirection;

    public bool IsShooting => shoot.ReadValue<float>() > 0;

    ShipMove shipMove;
    private void OnDisable()
    {
        if (rotation != null)
            rotation.performed -= GetLookDirection;
        if (shoot != null)
        {
            shoot.started -= OnShootStarted;
            shoot.canceled -= OnShootCanceled;
        }
        CancelInvoke(nameof(Shoot));
    }
    void Start()
    {
        cam = Camera.main;
        shipMove = GetComponent<ShipMove>();
        shipManager = GetComponentInChildren<ShipManager>();
        rotation = InputSystem.actions.FindAction("Aim");
        shoot = InputSystem.actions.FindAction("Shoot");
        
        rotation.performed += GetLookDirection;
        shoot.started += OnShootStarted;
        shoot.canceled += OnShootCanceled;

        weapons = shipManager.ShootBlocs;
        foreach (Bloc f in weapons)
        {
            shootPoints.Add(f.BulletSpawn);
        }
    }
    private void OnShootStarted(InputAction.CallbackContext ctx)
    {
        InvokeRepeating(nameof(Shoot), 0, shootRate / factor);
    }

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        CancelInvoke(nameof(Shoot));
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
        foreach (Transform shootPoint in shootPoints)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, transform.rotation * Quaternion.Euler(90, 0, 0));
            bullet.GetComponent<Projectile>().SetSpeed((bulletSpeed * factor) + shipMove.GetCurrentVelocity().magnitude);
        }
    }
    public void UpdateShootRate()
    {
        // Annule le tir en cours
        CancelInvoke(nameof(Shoot));
        // Si le bouton de tir est maintenu, relance le tir avec le nouveau facteur
        if (shoot.ReadValue<float>() > 0)
            InvokeRepeating(nameof(Shoot), 0, shootRate / factor);
    }
}