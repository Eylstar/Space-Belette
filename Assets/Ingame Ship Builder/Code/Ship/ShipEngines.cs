//using UnityEngine;

///// <summary>
///// Simulates thrusters of a ship by applying linear and angular forces.
///// This is based on the ship physics from https://github.com/brihernandez/UnityCommon/blob/master/Assets/ShipPhysics/ShipPhysics.cs
///// </summary>
//public class ShipEngines : MonoBehaviour
//{
//    // Horizontal axis strafe, vertical axis strafe, forward thrust
//    private Vector3 linearThrust = new Vector3(100.0f, 100.0f, 100.0f);
//    // Pitch, yaw and roll axis thrust
//    private Vector3 angularThrust = new Vector3(100.0f, 100.0f, 100.0f);

//    // Multiplier for all forces. Can be used to keep force numbers smaller and more readable
//    private const float FORCE_MULTIPLIER = 100.0f;

//    private Vector3 appliedLinearForce = Vector3.zero;
//    private Vector3 appliedAngularForce = Vector3.zero;
//    private Vector3 maxAngularForce;
//    private Rigidbody rbody;

//    // Engine kill controls
//    private float rBodyDrag;

//    public bool IsEngineOn
//    {
//        get { return isEngineOn; }
//        set { isEngineOn = value; }
//    }
//    private bool isEngineOn = true;

//    // Supercruise speed
//    public bool InSupercruise
//    {
//        get { return inSupercruise; }
//        set { inSupercruise = value; }
//    }
//    private bool inSupercruise = false;

//    // Keep a reference to the ship this is attached to just in case.
//    private Ship ship;

//    // Use this for initialization
//    void Awake()
//    {
//        rbody = GetComponent<Rigidbody>();
//        if (rbody == null)
//        {
//            Debug.LogWarning(name + ": ShipPhysics has no rigidbody.");
//        }

//        ship = GetComponent<Ship>();
//        linearThrust = ship.ShipModelInfo.LinearForce;
//        angularThrust = ship.ShipModelInfo.AngularForce;
//    }

//    private void Start()
//    {
//        rBodyDrag = rbody.linearDamping;
//        maxAngularForce = angularThrust * FORCE_MULTIPLIER;
//    }

//    void FixedUpdate()
//    {
//        if (rbody != null)
//        {
//            if(isEngineOn)
//                rbody.AddRelativeForce(appliedLinearForce, ForceMode.Force);

//            rbody.AddRelativeTorque(
//                ClampVector3(appliedAngularForce, -1 * maxAngularForce, maxAngularForce),
//                ForceMode.Force);
//        }
//    }

//    private void Update()
//    {
//        Vector3 linearInput, angularInput;

//        linearInput = new Vector3(ship.PlayerInput.strafe, 0, ship.PlayerInput.throttle);
//        angularInput = new Vector3(ship.PlayerInput.pitch, ship.PlayerInput.yaw, ship.PlayerInput.roll);
//        SetPhysicsInput(linearInput, angularInput);
//    }

//    /// <summary>
//    /// Sets the input for how much of linearForce and angularForce are applied
//    /// to the ship. Each component of the input vectors is assumed to be scaled
//    /// from -1 to 1, but is not clamped.
//    /// </summary>
//    private void SetPhysicsInput(Vector3 linearInput, Vector3 angularInput)
//    {
//        appliedLinearForce = MultiplyByComponent(linearInput, linearThrust) * FORCE_MULTIPLIER;
//        appliedAngularForce = MultiplyByComponent(angularInput, angularThrust) * FORCE_MULTIPLIER;
//    }

//    /// <summary>
//    /// Turns the main engine intertial dampening off or on, by disabling the linear drag on the ship.
//    /// </summary>
//    public void ToggleEngines(Light engineTorch)
//    {
//        isEngineOn = !isEngineOn;
//        if (!isEngineOn)
//        {
//            rbody.linearDamping = 0;
//            engineTorch.intensity = 0;
//        }
//        else
//        {
//            rbody.linearDamping = rBodyDrag;
//            engineTorch.intensity = 1.0f;
//        }
//    }

//    #region helper methods
//    /// <summary>
//    /// Returns a Vector3 where each component of Vector A is multiplied by the equivalent component of Vector B.
//    /// </summary>
//    private Vector3 MultiplyByComponent(Vector3 a, Vector3 b)
//    {
//        Vector3 ret;

//        ret.x = a.x * b.x;
//        ret.y = a.y * b.y;
//        ret.z = a.z * b.z;

//        return ret;
//    }

//    /// <summary>
//    /// Clamps vector components to a value between the minimum and maximum values given in min and max vectors.
//    /// </summary>
//    /// <param name="vector">Vector to be clamped</param>
//    /// <param name="min">Minimum vector components allowed</param>
//    /// <param name="max">Maximum vector components allowed</param>
//    /// <returns></returns>
//    private Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max)
//    {
//        return new Vector3(
//            Mathf.Clamp(vector.x, min.x, max.x),
//            Mathf.Clamp(vector.y, min.y, max.y),
//            Mathf.Clamp(vector.z, min.z, max.z)
//            );
//    }
//    #endregion helper methods
//}