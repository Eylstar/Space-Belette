using System;
using System.Collections;
using UnityEngine;

public class Chasing : Enemy
{
    [SerializeField] float distanceMinimumToPlayer = 20f;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(LookAtPlayer());
    }

    IEnumerator LookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(playerShip.transform.position - transform.position);
        Quaternion currentRotation = transform.rotation;
        float time = 0f;
        while (time < 1f)
        {
            transform.rotation = Quaternion.Slerp(currentRotation, rotation, time);
            time += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        StartCoroutine(LookAtPlayer());
    }

    protected override void Die()
    {
        base.Die();
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        //rb.linearVelocity = transform.forward.normalized * (moveSpeed * Time.fixedDeltaTime);
        // Check if the distance to the player is greater than the minimum distance
        float distanceToPlayer = Vector3.Distance(transform.position, playerShip.transform.position);
        if (distanceToPlayer > distanceMinimumToPlayer)
        {
            // If the distance is greater than the minimum, continue moving towards the player
            Vector3 directionToPlayer = (playerShip.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + directionToPlayer * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // If the distance is less than or equal to the minimum, decelerate fast
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime);
        }
    }
}   
