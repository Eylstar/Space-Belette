using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMove : MonoBehaviour
{
    InputAction movement;
    Vector2 moveDirection;
    
    [SerializeField] float speed = 10f;
    
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        movement = InputSystem.actions.FindAction("Move");
        movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        movement.canceled += _ => Move(Vector2.zero);
    }
    
    void Move(Vector2 direction)
    {
        moveDirection = direction;
    }
    
    void FixedUpdate()
    {
        Vector2 appliedMovement = moveDirection * (speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + new Vector3(appliedMovement.x, 0, appliedMovement.y));
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