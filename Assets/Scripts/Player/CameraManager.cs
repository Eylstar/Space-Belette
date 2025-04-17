using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float lerpStrength = 0.1f;
    Vector3 offset;
    
    [SerializeField] float MaxSize;
    float OriginalSize;
    float currentZoomFactor = 0f;
    [SerializeField] float zoomOutSpeed = 3f;
    [SerializeField] float zoomInSpeed = 1f;

    void Start()
    {
        offset = cam.transform.position - transform.position;
        OriginalSize = cam.orthographicSize;
    }
    
    void FixedUpdate()
    {
        Vector3 targetPosition = transform.position + offset;
        cam.transform.position = Vector3.Slerp(cam.transform.position, targetPosition, lerpStrength);
    }
    
    public void SetDezoomFactor(float speedratio)
    {
        float threshold = 0.75f;
        float targetZoomFactor = Mathf.InverseLerp(threshold, 1f, speedratio);
        
        float zoomSpeed = (targetZoomFactor > currentZoomFactor) ? zoomOutSpeed : zoomInSpeed;
        currentZoomFactor = Mathf.MoveTowards(currentZoomFactor, targetZoomFactor, Time.deltaTime * zoomSpeed);
        cam.orthographicSize = Mathf.Lerp(OriginalSize, MaxSize, currentZoomFactor);
    }
}