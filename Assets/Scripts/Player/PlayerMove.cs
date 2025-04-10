using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    Camera cam;
    
    [Header("Movement")]

    InputAction movement;
    InputAction sprint;
    InputAction jump;
    
    PlayerAnimationController animController;
    
    Vector2 moveDirection;
    Vector2 currentInputVector;
    Vector2 currentVelocity;
    
    float speed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float smoothInputAcceleration = 0.3f;
    [SerializeField] float smoothInputDecceleration = 0.3f;
    [SerializeField] float jumpForce = 5;
    
    Rigidbody rb;
    
    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();
        SetupInputs();
    }
    
    void SetupInputs()
    {
        movement = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");
        
        movement.performed += ctx => GetMoveVector(ctx.ReadValue<Vector2>());
        movement.canceled += _ => GetMoveVector(Vector2.zero);

        sprint.performed += _ => SetSpeed(sprintSpeed);
        sprint.canceled += _ => SetSpeed(walkSpeed);
        
        //jump.performed += _ => OnJump();
    }

    void Start()
    {
        SetSpeed(walkSpeed);
    }
    
    void SetSpeed(float newSpeed) => speed = newSpeed;
    void GetMoveVector(Vector2 direction) => moveDirection = direction;

    void FixedUpdate()
    {
        UpdatePosition();
        ApplyHorizontalDamping();
    }
    
    void UpdatePosition()
    {
        currentInputVector = Vector2.SmoothDamp(currentInputVector, moveDirection, ref currentVelocity, speed == sprintSpeed ? smoothInputAcceleration : smoothInputDecceleration);
        Vector3 move = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        move = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * move;
        if (move.magnitude < 0.05f) return;
        
        move.Normalize();
        rb.MoveRotation(Quaternion.LookRotation(move));
        rb.AddForce(move * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    
    void ApplyHorizontalDamping()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x *= 1 - (8f * Time.fixedDeltaTime);
        vel.z *= 1 - (8f * Time.fixedDeltaTime);
        rb.linearVelocity = vel;
    }
    
    void OnJump()
    {   
        if (!IsGrounded()) return;
        Debug.Log("Jump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animController.OnJump();
    }
    
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, 1.25f);
    }
    
    void OnEnable()
    {
        movement.Enable();
        sprint.Enable();
        jump.Enable();
    }
    
    void OnDisable()
    {
        movement.Disable();
        sprint.Disable();
        jump.Disable();
    }
    
    public float GetAnimSpeed() { return moveDirection.magnitude; }
    public bool IsSprinting() { return speed == sprintSpeed; }
}
