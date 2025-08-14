using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPContainer : MonoBehaviour
{
    GameObject player;
    
    public float experience = 10;
    public bool triggered = false;
    
    public void SetExperience(float xp)
    {
        experience = xp;
    }
    
    public void SetPlayer(GameObject playerObject)
    {
        player = playerObject;
    }

    public void StartMoving()
    {
        if (player != null)
            StartCoroutine(MoveTowardsPlayer());
    }
    
    IEnumerator MoveTowardsPlayer()
    {
        float duration = 1f; // Duration of the movement
        float elapsedTime = 0f;
        
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            Vector3 targetPosition = player.transform.position;
            transform.LookAt(targetPosition);
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = player.transform.position;
        Destroy(gameObject);
    }
}
