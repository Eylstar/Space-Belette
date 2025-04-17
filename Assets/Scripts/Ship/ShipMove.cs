using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMove : MonoBehaviour
{
    CameraManager camManager;
    
    InputAction movement;
    Vector2 moveDirection;
    Vector2 currentVelocity;
    
    [SerializeField] float speed = 10f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 3f;
    [SerializeField] float maxSpeed = 20f;
    
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        movement = InputSystem.actions.FindAction("Move");
        movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        movement.canceled += _ => Move(Vector2.zero);

        camManager = GetComponent<CameraManager>();
    }
    
    void Move(Vector2 direction)
    {
        moveDirection = direction;
    }
    
    void FixedUpdate()
    {
        Vector2 targetVelocity = moveDirection * speed;

        float accelFactor;
        if (moveDirection != Vector2.zero)
        {
            float t = Vector2.Distance(currentVelocity, targetVelocity) / maxSpeed;
            
            float directionDot = Vector2.Dot(currentVelocity.normalized, moveDirection.normalized);
            bool isChangingDirection = directionDot < 0.5f;
            
            accelFactor = acceleration * (isChangingDirection ? 4f : Mathf.SmoothStep(0f, 1f, t));
        }
        else
        {
            accelFactor = deceleration;
        }
        
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, accelFactor * Time.fixedDeltaTime);
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, maxSpeed);
        
        Vector3 movement = new Vector3(currentVelocity.x, 0, currentVelocity.y) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        
        camManager.SetDezoomFactor(currentVelocity.magnitude / maxSpeed);
    }
    
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    
    void OnEnable()
    {
        movement.Enable();
    }
    
    void OnDisable()
    {
        movement.Disable();
    }
}