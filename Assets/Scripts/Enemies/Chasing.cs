using System;
using System.Collections;
using UnityEngine;

public class Chasing : Enemy
{
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
        rb.linearVelocity = transform.forward.normalized * (moveSpeed * Time.fixedDeltaTime);
    }
}
