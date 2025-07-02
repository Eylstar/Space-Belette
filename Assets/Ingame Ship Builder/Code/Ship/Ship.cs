using UnityEngine;

/// <summary>
/// Ties all the primary ship components together
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShipCockpitControls))]
[RequireComponent(typeof(ShipEngines))]
public class Ship : MonoBehaviour
{
    #region ship components
    // Player controls
    public ShipCockpitControls PlayerInput
    {
        get { return playerInput; }
    }
    private ShipCockpitControls playerInput;

    // Ship rigidbody physics
    public ShipEngines Thrusters
    {
        get { return thrusters; }
    }
    private ShipEngines thrusters;
    #endregion ship components

    public bool UsingMouseInput
    {
        set { playerInput.useMouseInput = value; }
        get { return playerInput.useMouseInput; }
    }

    public Vector3 Velocity
    {
        get { return rbody.linearVelocity; }
    }

    public float Throttle
    {
        get { return playerInput.throttle; }
    }

    public bool InSupercruise {
        get { return thrusters.InSupercruise; }
        set { thrusters.InSupercruise = value;    }
    }

    private Rigidbody rbody;

    // Editor assignable properties
    [Tooltip("The model info holder which contains all data for this ship model")]
    public ModelInfo ShipModelInfo;
    [Tooltip("Is this ship currently piloted by the player")]
    public bool isPlayerShip = false;

    public static Ship PlayerShip;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<ShipCockpitControls>();
        thrusters = GetComponent<ShipEngines>();

        if (playerInput == null || thrusters == null)
        {
            Debug.LogError("Component not found on ship " + name);
        }
    }

    private void Start()
    {
        if (isPlayerShip)
            PlayerShip = this;
    }

    /// <summary>
    /// Turns the engine on and off.
    /// </summary>
    public void ToggleEngine(Light engineTorch)
    {
        thrusters.ToggleEngines(engineTorch);
    }

}
