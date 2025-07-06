using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracking : MonoBehaviour
{
    public Transform target;              // Target
    public GameObject targetObject;
    public Transform turretMount;         // Y-axis rotation (horizontal tracking)
    public Transform turretHead;          // X-axis rotation (elevation)
    public Transform firePoint;           // Firing point
    public float range = 100f;            // Detection range
    public float rotationSpeed = 5f;
    private float bulletSpeed;
    private bool holdFire = true;
    public bool predictOn = true;
    string[] enemyTags = { "Enemy", "Tank" };

    // Velocity prediction
    private Vector3 lastPosition;
    public Vector3 calculatedVelocity { get; private set; }

    private void Start()
    {
        lastPosition = transform.position;
        calculatedVelocity = Vector3.zero;

        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        FireControl fireControl = GetComponent<FireControl>();
        if (fireControl != null)
        {
            bulletSpeed = fireControl.bulletSpeed;
        }
        else
        {
            Debug.LogWarning("BulletSetting script not found!");
        }
    }

    void Update()
    {
        if (target == null || turretMount == null || turretHead == null)
        {
            holdFire = true;
            return;
        }
        holdFire = false;

        // Calculate direction vector from turret to target
        Vector3 predictedPos = target.position;
        if (predictOn)
        {
            predictedPos = PredictFuturePosition(target, bulletSpeed);
        }

        Vector3 direction = predictedPos - firePoint.position;

        // 1. Rotate turretMount horizontally (Y axis)
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion mountRotation = Quaternion.LookRotation(flatDir);
            turretMount.rotation = Quaternion.Slerp(turretMount.rotation, mountRotation, Time.deltaTime * rotationSpeed);
        }

        // 2. Rotate turretHead vertically (X axis)
        Vector3 verticalDir = new Vector3(direction.x, direction.y, direction.z);
        if (verticalDir.sqrMagnitude > 0.001f)
        {
            Quaternion headRotation = Quaternion.LookRotation(verticalDir);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, headRotation, Time.deltaTime * rotationSpeed * 0.2f);
        }
    }

    Vector3 PredictFuturePosition(Transform target, float projectileSpeed)
    {
        calculatedVelocity = (target.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = target.transform.position;
        Vector3 targetVelocity = calculatedVelocity;

        Vector3 toTarget = target.position - firePoint.position;
        float distance = toTarget.magnitude;
        float timeToHit = distance / projectileSpeed;

        return target.position + targetVelocity * timeToHit;
    }


    void UpdateTarget()
    {
        GameObject[] enemies = FindGameObjectsWithTags(enemyTags);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < range)
            {
                if (target == null)
                {
                    // If no current target, lock on the first enemy in range
                    target = enemy.transform;
                    targetObject = enemy;
                    return;
                }

                // If current target is gone or out of range, find the closest one
                if (target == null || Vector3.Distance(transform.position, target.position) > range)
                {
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    }
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
            targetObject = nearestEnemy;
        }
        else if (target != null && Vector3.Distance(transform.position, target.position) > range)
        {
            target = null;
            targetObject = null;
        }

        if (enemies.Length == 0)
        {
            target = null;
            targetObject = null;
        }
    }

    public bool IsFire()
    {
        return !holdFire;
    }

    /// <summary>
    /// Find all objects with the given tags
    /// </summary>
    public GameObject[] FindGameObjectsWithTags(string[] tags)
    {
        List<GameObject> allObjects = new List<GameObject>();

        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            allObjects.AddRange(objects);
        }

        return allObjects.ToArray();
    }
}
