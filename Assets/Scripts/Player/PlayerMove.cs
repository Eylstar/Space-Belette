using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Camera cam;
    private Rigidbody rb;
    private PlayerAnimationController animController;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = .3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float smoothInputAcceleration = 0.3f;
    [SerializeField] private float smoothInputDecceleration = 0.3f;

    private InputAction movement;
    private InputAction sprint;
    private InputAction jump;

    private Vector2 moveDirection;
    private Vector2 currentInputVector;
    private Vector2 currentVelocity;
    private float speed;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        SetupInputs();
    }

    private void SetupInputs()
    {
        movement = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");

        movement.performed += ctx => GetMoveVector(ctx.ReadValue<Vector2>());
        movement.canceled += _ => GetMoveVector(Vector2.zero);

        sprint.performed += _ => SetSpeed(sprintSpeed);
        sprint.canceled += _ => SetSpeed(walkSpeed);

        jump.performed += _ => OnJump();
    }

    private void Start()
    {
        SetSpeed(walkSpeed);
    }

    private void GetMoveVector(Vector2 direction) => moveDirection = direction;
    private void SetSpeed(float newSpeed) => speed = newSpeed;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        currentInputVector = Vector2.SmoothDamp(currentInputVector, moveDirection, ref currentVelocity,
            speed == sprintSpeed ? smoothInputAcceleration : smoothInputDecceleration);

        Vector3 move = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        move = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * move;
        if (move.magnitude < 0.05f) return;

        move.Normalize();
        Vector3 force = move * speed;
        rb.AddForce(force, ForceMode.VelocityChange);

        if (force != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            rb.MoveRotation(targetRotation);
        }
    }

    private void OnJump()
    {
        if (!IsGrounded()) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animController?.OnJump();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.2f);
    }

    private void OnEnable()
    {
        movement.Enable();
        sprint.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        sprint.Disable();
        jump.Disable();
    }

    // Public utils for animation etc.
    public float GetAnimSpeed() => moveDirection.magnitude;
    public bool IsSprinting() => speed == sprintSpeed;
}
