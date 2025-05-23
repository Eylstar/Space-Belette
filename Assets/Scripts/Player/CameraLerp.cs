using Unity.Cinemachine;
using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    CinemachineCamera cam;
    [SerializeField] float lerpStrength = 0.1f;
    [SerializeField] Vector3 offset;
    
    [SerializeField] float MaxSize;
    float OriginalSize;
    float currentZoomFactor = 0f;
    [SerializeField] float zoomOutSpeed = 3f;
    [SerializeField] float zoomInSpeed = 1f;

    void Start()
    {
        cam = FindFirstObjectByType<CameraManager>().shipCinemachineCam.GetComponent<CinemachineCamera>();
        //offset = cam.transform.position - transform.position;
        OriginalSize = cam.Lens.OrthographicSize;
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
        cam.Lens.OrthographicSize = Mathf.Lerp(OriginalSize, MaxSize, currentZoomFactor);
    }
}