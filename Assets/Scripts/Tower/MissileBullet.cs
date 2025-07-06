using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class MissileBullet : MonoBehaviour, IDamageable
{
    public Transform target;               // Target
    public float lifetime = 10f;
    public int damage = 10;
    public float speed = 20f;
    public float rotateSpeed = 5f;
    public float delayBeforeTracking = 0.5f; // Delay before tracking starts (seconds)
    public float range = 100f;              // Target search range

    public GameObject explosionEffectPrefab; // Explosion effect prefab
    public float explosionRadius = 5f;
    public float explosionForce = 500f;

    public AudioClip boomSound;
    public float volume = 0.8f;

    private bool hasExploded = false;

    string[] enemyTags = { "Enemy", "Tank" }; // Enemy tags

    private bool isTracking = false;
    private Rigidbody rb;

    void Start()
    {
        Invoke(nameof(StartTracking), delayBeforeTracking);
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

        Destroy(gameObject, lifetime); // Destroy after lifespan if no impact

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isTracking && target != null)
        {
            // Calculate direction to target
            Vector3 dir = (target.position - transform.position).normalized;

            // Rotate toward the target smoothly
            Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, rotateSpeed * Time.fixedDeltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            // Always move forward
            rb.velocity = transform.forward * speed;
        }
    }

    void StartTracking()
    {
        isTracking = true;
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
                    // Lock on the first valid target if no current target
                    target = enemy.transform;
                    return;
                }

                // If the current target is gone or out of range, find the closest one
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
            target = nearestEnemy.transform;
        else if (target != null && Vector3.Distance(transform.position, target.position) > range)
            target = null;
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void Explode()
    {
        hasExploded = true;

        // Spawn explosion VFX
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f); // Destroy effect after 2 seconds
        }

        // Apply explosion force and damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Apply damage to nearby enemies
            if (nearby.CompareTag("Enemy"))
            {
                Debug.Log("Emotional Damaged!");
                SoldierBehavior health = nearby.gameObject.GetComponent<SoldierBehavior>();
                health.TakeDamage(damage);
            }

            if (nearby.CompareTag("Tank"))
            {
                Debug.Log("Emotional Damaged!");
                TankBossBehavior health = nearby.gameObject.GetComponent<TankBossBehavior>();
                health.TakeDamage(damage);
            }
        }

        PlayBoomSound();
        Destroy(gameObject); // Destroy missile object
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return; // Prevent multiple explosions

        if (!other.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    GameObject[] FindGameObjectsWithTags(string[] tags)
    {
        List<GameObject> allObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            allObjects.AddRange(objects);
        }
        return allObjects.ToArray();
    }

    void PlayBoomSound()
    {
        if (boomSound == null) return;

        // Create a temporary GameObject for sound playback
        GameObject audioObj = new GameObject("TempAudio");
        audioObj.transform.position = transform.position;

        // Add AudioSource component
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = boomSound;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioObj, boomSound.length); // Destroy audio object after playback
    }
}
