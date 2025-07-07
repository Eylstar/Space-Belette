//using UnityEngine;
//using System;
//using System.Collections;

///// <summary>
///// Deals with player input from keyboard, mouse and other input sources
///// </summary>
//public class ShipCockpitControls : MonoBehaviour
//{
//    [Tooltip("When using Keyboard/Joystick input, should roll be added to horizontal stick movement. This is a common trick in traditional space sims to help ships roll into turns and gives a more plane-like feeling of flight.")]
//    public bool addRoll = true;
//    [Tooltip("When true, the mouse and mousewheel are used for ship input and A/D can be used for strafing like in many arcade space sims.\n\nOtherwise, WASD/Arrows/Joystick + R/T are used for flying, representing a more traditional style space sim.")]
//    public bool useMouseInput = true;

//    [Range(-1, 1)]
//    public float pitch;
//    [Range(-1, 1)]
//    public float yaw;
//    [Range(-1, 1)]
//    public float roll;
//    [Range(-1, 1)]
//    public float strafe;
//    [Range(-0.1f, 1)]
//    public float throttle;

//    // How quickly the throttle reacts to input
//    private const float THROTTLE_SPEED = 0.5f;
//    // Keep a reference to the ship this is attached to just in case
//    private Ship ship;
//    // Reference to engine light used to visually represent supercruise speed
//    private Light engineTorch;

//    private void Awake()
//    {
//        ship = GetComponent<Ship>();
//        engineTorch = GetComponentInChildren<Light>();
//    }

//    private void Update()
//    {
//        if (useMouseInput)
//        {
//            strafe = Input.GetAxis("Horizontal");
//            SetStickSteeringUsingMouse();
//            if (!ship.InSupercruise)
//            {
//                UpdateMouseWheelThrottle();
//                UpdateKeyboardThrottle(KeyCode.W, KeyCode.S);
//            }
//        }
//        else
//        {
//            pitch = Input.GetAxis("Vertical");
//            yaw = Input.GetAxis("Horizontal");

//            if (addRoll)
//                roll = -Input.GetAxis("Horizontal") * 0.5f;

//            strafe = 0.0f;
//            UpdateKeyboardThrottle(KeyCode.R, KeyCode.F);
//        }

//        if(Input.GetKey(KeyCode.Q))
//        {
//            roll = 1;
//        }
//        else if (Input.GetKey(KeyCode.E))
//        {
//            roll = -1;
//        }
//        else
//        {
//            roll = 0;
//        }

//        if (Input.GetKeyDown(KeyCode.Backspace))
//        {
//            throttle = 0.0f;
//        }

//        if (Input.GetKeyDown(KeyCode.Z))
//        {
//            ship.ToggleEngine(engineTorch);
//        }

//        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
//        {
//            ToggleSupercruise();
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            useMouseInput = !useMouseInput;
//        }

//    }

//    /// <summary>
//    /// Engages supercruise engines, which provide a much higher output (300%)
//    /// </summary>
//    public void ToggleSupercruise()
//    {
//        // Make sure the engine is on
//        if (!ship.Thrusters.IsEngineOn) { 
//            ship.ToggleEngine(engineTorch);
//        }

//        ship.InSupercruise = !ship.InSupercruise;

//        if (ship.InSupercruise) { 
//            StartCoroutine(EngageSupercruise());
//            if(engineTorch != null)
//                engineTorch.intensity = 5.0f;
//        }
//        if (!ship.InSupercruise)
//        {
//            throttle = 1f;
//            if (engineTorch != null)
//                engineTorch.intensity = 1.0f;
//        }
//    }

//    /// <summary>
//    /// Accelerate ship to 3x max throttle gradually
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator EngageSupercruise()
//    {
//        while(throttle < 3.0f)
//        {
//            if (!ship.InSupercruise)
//                yield break;

//            throttle = Mathf.MoveTowards(throttle, 3.0f, Time.deltaTime * THROTTLE_SPEED);

//            yield return null;
//        }
//        yield return null;
//    }


//    /// <summary>
//    /// Freelancer style mouse controls. This uses the mouse to simulate a virtual joystick.
//    /// When the mouse is in the center of the screen, this is the same as a centered stick.
//    /// </summary>
//    private void SetStickSteeringUsingMouse()
//    {
//        Vector3 mousePos = Input.mousePosition;

//        // Figure out most position relative to center of screen.
//        // (0, 0) is center, (-1, -1) is bottom left, (1, 1) is top right.      
//        pitch = (mousePos.y - (Screen.height * 0.5f)) / (Screen.height * 0.5f);
//        yaw = (mousePos.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f);

//        // Make sure the values don't exceed limits.
//        pitch = -Mathf.Clamp(pitch, -1.0f, 1.0f);
//        yaw = Mathf.Clamp(yaw, -1.0f, 1.0f);
//    }

//    /// <summary>
//    /// Uses R and F to raise and lower the throttle.
//    /// </summary>
//    private void UpdateKeyboardThrottle(KeyCode increaseKey, KeyCode decreaseKey)
//    {
//        if(ship.InSupercruise && Input.GetKey(decreaseKey))
//        {
//            throttle = 1.0f;
//            ship.InSupercruise = false;
//            return;
//        }
//        if (ship.InSupercruise)
//            return;

//        float target = throttle;

//        if (Input.GetKey(increaseKey))
//            target = 1.0f;
//        else if (Input.GetKey(decreaseKey))
//            target = -0.1f;

//        throttle = Mathf.MoveTowards(throttle, target, Time.deltaTime * THROTTLE_SPEED);
//    }

//    /// <summary>
//    /// Uses the mouse wheel to control the throttle.
//    /// </summary>
//    private void UpdateMouseWheelThrottle()
//    {
//        throttle += Input.GetAxis("Mouse ScrollWheel");
//        throttle = Mathf.Clamp(throttle, -0.1f, 1.0f);
//    }
//}