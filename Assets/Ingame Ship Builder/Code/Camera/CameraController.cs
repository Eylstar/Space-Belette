//using System.Collections;
//using UnityEngine;
//[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
//public class CameraController : MonoBehaviour
//{
//    [HideInInspector] public Transform target;

//    public enum CameraState
//    {
//        Chase, Orbit
//    }

//    public float distance;
//    public float xSpeed;
//    public float ySpeed;
//    public float yMinLimit;
//    public float yMaxLimit;
//    public float distanceMin;
//    public float distanceMax;
//    public float smoothTime;
//    public float cameraFollowSpeed = 15f;

//    private float rotationYAxis = 0.0f;
//    private float rotationXAxis = 0.0f;
//    private float velocityX = 0.0f;
//    private float velocityY = 0.0f;

//    public CameraState State;

//    [HideInInspector]
//    public bool AutoOrbit = false;

//    // Chase camera
//    public float rotateSpeed = 90.0f;
//    private Vector3 startOffset;

//    void Start()
//    {
//        Vector3 angles = transform.eulerAngles;
//        rotationYAxis = angles.y;
//        rotationXAxis = angles.x;
//        // Make the rigid body not change rotation
//        if (GetComponent<Rigidbody>())
//        {
//            GetComponent<Rigidbody>().freezeRotation = true;
//        }

//        if (Ship.PlayerShip == null)
//            Debug.Log("Player ship is null.");
//        else if (target == null)
//        {
//            startOffset = new Vector3(0, 5, -Ship.PlayerShip.ShipModelInfo.CameraOffset);
//            target = Ship.PlayerShip.transform;
//        }
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.H))
//        {
//            if (State == CameraState.Orbit)
//                State = CameraState.Chase;
//            else
//                State = CameraState.Orbit;
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            ToggleFlightMode();
//        }

//    }

//    public void ToggleFlightMode()
//    {
//        if (State == CameraState.Orbit)
//        {
//            AutoOrbit = !AutoOrbit;
//            Ship.PlayerShip.UsingMouseInput = !Ship.PlayerShip.UsingMouseInput;

//        }
//    }

//    void FixedUpdate()
//    {
//        if (!target)
//            return;

//        if (State == CameraState.Orbit)
//        {
//            // Orbit Camera
//            if (!AutoOrbit)
//            {
//                velocityX = 0;
//                velocityY = 0;
//                if (Input.mousePosition.x < Screen.width * 0.25f)
//                {
//                    velocityX = (Input.mousePosition.x - 0.25f * Screen.width) * xSpeed * (distance / 10) / (0.25f * Screen.width) * 0.02f;
//                }
//                if (Input.mousePosition.x > Screen.width * 0.75f)
//                {
//                    velocityX = (Input.mousePosition.x - 0.75f * Screen.width) * xSpeed * (distance / 10) / (0.25f * Screen.width) * 0.02f;
//                }
//                if (Input.mousePosition.y < Screen.height * 0.25f)
//                {
//                    velocityY = (Input.mousePosition.y - 0.25f * Screen.height) * ySpeed * (distance / 10) / (0.25f * Screen.height) * 0.02f;
//                }
//                if (Input.mousePosition.y > Screen.height * 0.75f)
//                {
//                    velocityY = (Input.mousePosition.y - 0.75f * Screen.height) * ySpeed * (distance / 10) / (0.25f * Screen.height) * 0.02f;
//                }
//            }
//            else
//            {
//                velocityX = 0.1f;
//            }

//            rotationYAxis += velocityX;
//            rotationXAxis -= velocityY;
//            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
//            Quaternion rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);

//            if (Input.GetKeyDown(KeyCode.KeypadPlus))
//            {
//                distance = Mathf.Clamp(distance - 10, distanceMin, distanceMax);
//            }
//            if (Input.GetKeyDown(KeyCode.KeypadMinus))
//            {
//                distance = Mathf.Clamp(distance + 10, distanceMin, distanceMax);
//            }

//            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
//            Vector3 position = rotation * negDistance + target.position;

//            transform.rotation = rotation;
//            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * cameraFollowSpeed);
//            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
//            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
//        }
//        else
//        {
//            // Chase camera
//            transform.position = Vector3.Lerp(
//                transform.position,
//                target.TransformPoint(startOffset),
//                Time.deltaTime * cameraFollowSpeed);

//            transform.rotation = Quaternion.Slerp(
//                transform.rotation,
//                target.rotation,
//                rotateSpeed * Time.deltaTime);
//        }
//    }

//    private float ClampAngle(float angle, float min, float max)
//    {
//        if (angle < -360F)
//            angle += 360F;
//        if (angle > 360F)
//            angle -= 360F;
//        return Mathf.Clamp(angle, min, max);
//    }

//    public void SetTargetShip(Ship newTarget)
//    {
//        target = newTarget.transform;
//        startOffset = new Vector3(0, 5, -newTarget.ShipModelInfo.CameraOffset);
//    }

//}