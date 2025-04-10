using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float lerpStrength = 0.1f;
    Vector3 offset;
    
    void Start()
    {
        offset = cam.transform.position - transform.position;
    }
    
    void FixedUpdate()
    {
        Vector3 targetPosition = transform.position + offset;
        cam.transform.position = Vector3.Slerp(cam.transform.position, targetPosition, lerpStrength);
    }
}