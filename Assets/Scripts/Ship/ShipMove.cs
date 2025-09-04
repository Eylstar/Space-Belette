using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMove : MonoBehaviour
{
    CameraLerp camLerp;
    
    InputAction movement;
    Vector2 moveDirection;
    Vector2 currentVelocity;

    [SerializeField] float baseSpeed = 10f;
    [SerializeField] float baseAcceleration = 10f;
    [SerializeField] float baseDeceleration = 3f;
    [SerializeField] float baseMaxSpeed = 20f;
    float multiplier = 1f;

    float speed => baseSpeed;
    float acceleration => baseAcceleration;
    float deceleration => baseDeceleration;
    float maxSpeed => baseMaxSpeed;

    Rigidbody rb;

    
    
    
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        movement = InputSystem.actions.FindAction("Move");
        movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        movement.canceled += _ => Move(Vector2.zero);

        camLerp = GetComponent<CameraLerp>();
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
            float t = Vector2.Distance(currentVelocity, targetVelocity) / speed;
            
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
        
        Vector3 m = new Vector3(currentVelocity.x, 0, currentVelocity.y) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + m);
        
        camLerp.SetDezoomFactor(currentVelocity.magnitude / maxSpeed);
    }
    
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    public void SpeedModificator(float mult) 
    {
        baseMaxSpeed = baseMaxSpeed + ((baseMaxSpeed*mult)/100);
    }
}