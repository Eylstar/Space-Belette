using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Camera cam;
    Vector3 offset;
    
    void Start()
    {
        cam = Camera.main;
        offset = cam.transform.position - transform.position;
    }
    
    void LateUpdate()
    {
        Vector3 targetPosition = transform.position + offset;
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, 0.1f);
    }
}