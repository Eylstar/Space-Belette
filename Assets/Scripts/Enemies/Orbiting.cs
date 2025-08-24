using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitingEnemy : Enemy
{
    [SerializeField] float orbitDistance = 15f;
    [SerializeField] float orbitSpeed = 5f;
    
    [SerializeField] float regroupRadius = 30f;
    [SerializeField] float regroupPointOffset = 40f;
    [SerializeField] float regroupTime = 2f;
    
    [SerializeField] float angleLerpSpeed = 5f; 
    [SerializeField] float reorganizeDelay = 0.5f;
    
    [SerializeField] float groupFireRate = 2f;
    [SerializeField] float fireOffset = 0.2f;
    
    [SerializeField] int maxFireCyclesBeforeRush = 3;
    [SerializeField] float rushSpeedMultiplier = 2f;

    private Vector3 regroupPoint;
    private Vector3 orbitCenter;
    private bool isRegrouping = true;
    private bool isRushing = false;
    private float regroupTimer = 0f;

    private float currentAngle;
    private float targetAngle;
    private float lastGroupChangeTime;

    private int lastGroupCount = -1;
    private int myIndex = 0;

    bool canUpdate = false;

    protected override void Start()
    {
        base.Start();
        Invoke(nameof(ForceUpdateGroupInfo), 0.2f);
        currentAngle = targetAngle;
    }
    
    private void ForceUpdateGroupInfo()
    {
        List<OrbitingEnemy> groupMembers = enemiesManager
            .GetInRangeEnemies(transform.position, regroupRadius)
            .OfType<OrbitingEnemy>()
            .OrderBy(e => e.GetInstanceID())
            .ToList();

        lastGroupCount = groupMembers.Count;
        myIndex = groupMembers.IndexOf(this);

        targetAngle = (360f / groupMembers.Count) * myIndex;

        Vector3 offset = new Vector3(
            Mathf.Cos(targetAngle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(targetAngle * Mathf.Deg2Rad)
        ) * regroupPointOffset;

        regroupPoint = playerShip.transform.position + offset;
        canUpdate = true;
    }

    protected override void Update()
    {
        if (!canUpdate) return;

        if (isRushing) return;

        if (!isRegrouping)
        {
            base.Update();
        }
    }

    void FixedUpdate()
    {
        if (playerShip == null) return;
        if (!canUpdate) return;

        CheckForGroupChanges();

        if (isRegrouping)
        {
            regroupTimer += Time.fixedDeltaTime;
            MoveTowards(regroupPoint, moveSpeed);

            if (Vector3.Distance(transform.position, regroupPoint) < 1.5f || regroupTimer > regroupTime)
                isRegrouping = false;
        }
        else if (isRushing)
        {
            MoveTowards(playerShip.transform.position, moveSpeed * rushSpeedMultiplier);
        }
        else
        {
            orbitCenter = playerShip.transform.position;
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, angleLerpSpeed * Time.fixedDeltaTime);

            float timeAngle = (Time.time * orbitSpeed * 10f) + currentAngle;

            Vector3 offset = new Vector3(
                Mathf.Cos(timeAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(timeAngle * Mathf.Deg2Rad)
            ) * orbitDistance;

            Vector3 targetPos = orbitCenter + offset;
            MoveTowards(targetPos, moveSpeed);
        }
    }

    private void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        rb.MovePosition(transform.position + dir * speed * Time.fixedDeltaTime);

        Quaternion lookRot = Quaternion.LookRotation(playerShip.transform.position - transform.position);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime));
    }

    private void CheckForGroupChanges()
    {
        List<OrbitingEnemy> groupMembers = enemiesManager
            .GetInRangeEnemies(transform.position, regroupRadius)
            .OfType<OrbitingEnemy>()
            .OrderBy(e => e.GetInstanceID())
            .ToList();

        if (groupMembers.Count != lastGroupCount)
        {
            lastGroupCount = groupMembers.Count;
            lastGroupChangeTime = Time.time;
        }

        myIndex = groupMembers.IndexOf(this);

        if (Time.time - lastGroupChangeTime >= reorganizeDelay)
        {
            targetAngle = (360f / groupMembers.Count) * myIndex;

            Vector3 offset = new Vector3(
                Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(targetAngle * Mathf.Deg2Rad)
            ) * regroupPointOffset;

            regroupPoint = playerShip.transform.position + offset;
        }
    }
}
