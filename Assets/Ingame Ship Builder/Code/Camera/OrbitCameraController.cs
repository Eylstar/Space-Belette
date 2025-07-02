using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [HideInInspector] public Transform target;

    public float distance;
    public float xSpeed;
    public float ySpeed;
    public float yMinLimit;
    public float yMaxLimit;
    public float distanceMin;
    public float distanceMax;
    public float smoothTime;
    public float cameraFollowSpeed = 15f;

    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;
    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    // Chase camera
    public float rotateSpeed = 90.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        if (Ship.PlayerShip == null)
            Debug.Log("Player ship is null.");
        else if (target == null)
        {
            target = Ship.PlayerShip.transform;
        }
    }

    void FixedUpdate()
    {
        if (!target)
            return;

        velocityX = 0;
        velocityY = 0;
        if (Input.GetMouseButton(1))
        {
            velocityX = (Input.mousePosition.x - 0.5f * Screen.width) * xSpeed * (distance / 10) / (0.5f * Screen.width) * 0.02f;
            velocityY = (Input.mousePosition.y - 0.5f * Screen.height) * ySpeed * (distance / 10) / (0.5f * Screen.height) * 0.02f;
        }

        rotationYAxis += velocityX;
        rotationXAxis -= velocityY;
        rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            distance = Mathf.Clamp(distance - 10, distanceMin, distanceMax);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            distance = Mathf.Clamp(distance + 10, distanceMin, distanceMax);
        }

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * cameraFollowSpeed);
        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

}