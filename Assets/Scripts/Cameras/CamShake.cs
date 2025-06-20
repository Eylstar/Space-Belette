using System.Collections;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public static CamShake instance;
    Camera cam;
    
    float currentShakeDuration = 0f;
    float shakeMagnitude = 0f;
    Coroutine routine;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        cam = Camera.main;
    }
    
    public void ShakeSmallEnemyKill()
    {
        Shake(0.15f, 0.2f);
    }
    
    public void ShakeMediumShipHit()
    {
        Shake(0.2f, 0.1f);
    }
    
    public void Shake(float duration, float magnitude)
    {
        currentShakeDuration = Mathf.Max(currentShakeDuration, duration);
        shakeMagnitude = magnitude;
        if (routine == null)
            routine = StartCoroutine(ShakeCoroutine());
    }
    
    IEnumerator ShakeCoroutine()
    {
        while (currentShakeDuration > 0f)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cam.transform.localPosition = new Vector3(x, 0, y);

            currentShakeDuration -= Time.deltaTime;
            yield return null;
        }
        
        cam.transform.localPosition = Vector3.zero;
        routine = null;
    }
}
