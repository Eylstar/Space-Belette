using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMove : MonoBehaviour
{
    CameraLerp camLerp;
    
    InputAction movement;
    Vector2 moveDirection;
    Vector2 currentVelocity;

    [SerializeField] float baseSpeed = 10f;
    [SerializeField] float baseAcceleration = 5f;
    [SerializeField] float baseDeceleration = 3f;
    [SerializeField] float baseMaxSpeed = 20f;
    float multiplier = 10f;

    float speed => baseSpeed * multiplier;
    float acceleration => baseAcceleration * multiplier;
    float deceleration => baseDeceleration * multiplier;
    float maxSpeed => baseMaxSpeed * multiplier;

    public bool ZigzagActive = false;
    public float zigzagAmplitude;
    public float zigzagFrequency;
    private float zigzagTimer = 0f;

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

        // Zigzag même à l'arrêt
        if (ZigzagActive)
        {
            zigzagTimer += Time.fixedDeltaTime * zigzagFrequency;
            float offset = Mathf.Sin(zigzagTimer) * zigzagAmplitude;

            // Si le joueur ne donne pas de direction, on prend "en avant" (par défaut vers Z+)
            Vector3 forward = transform.forward;
            Vector2 baseDir = moveDirection != Vector2.zero
                ? moveDirection.normalized
                : new Vector2(forward.x, forward.z).normalized;
            Vector2 perp = new Vector2(-baseDir.y, baseDir.x).normalized;
            targetVelocity += perp * offset;
        }
        else
        {
            zigzagTimer = 0f;
        }

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
        
        camLerp.SetDezoomFactor(currentVelocity.magnitude / maxSpeed);
    }
    
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    public void SpeedModificator(float mult) 
    {
        multiplier = mult;
        Debug.Log($"Max Speed : {maxSpeed}");
    }
}