using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour // Removed unused ,IDamageable
{
    public float lifetime = 3f;
    public int damage = 5; // Default damage set to 5

    public AudioClip boomSound; // Impact sound
    public float volume = 0.8f;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"MachineGunBullet '{name}' has a Collider not set as IsTrigger. Auto-setting it. Please check the prefab.", this);
            col.isTrigger = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }

        Destroy(gameObject, lifetime); // Auto-destroy after lifetime expires
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void OnTriggerEnter(Collider other)
    {
        // Try to get HealthBase from the hit object or its parent
        HealthBase enemyHealth = other.GetComponent<HealthBase>();
        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<HealthBase>();
        }

        bool hitValidTarget = false; // Mark whether a valid target was hit

        // If we found a HealthBase and it's not already dead
        if (enemyHealth != null && !other.CompareTag("DeadEnemy"))
        {
            // Apply damage based on tag
            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"Laser bullet hit soldier: {other.name}");
                enemyHealth.TakeDamage(damage);
                hitValidTarget = true;
            }
            else if (other.CompareTag("Tank"))
            {
                Debug.Log($"Laser bullet hit tank: {other.name}");
                // Fixed: removed /10 so tanks take full damage
                enemyHealth.TakeDamage(damage);
                hitValidTarget = true;
            }
        }

        // Only play sound and optionally destroy if a valid target was hit
        if (hitValidTarget)
        {
            PlayBoomSound();
            /*Destroy(gameObject); // Uncomment to destroy bullet immediately after hit */
        }

        // If you want bullets to be destroyed by hitting other objects (e.g., walls),
        // you can add more conditions here:
        // else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        // {
        //     PlayBoomSound();
        //     Destroy(gameObject);
        // }

        // If no valid target is hit, bullet keeps flying until its lifetime expires
    }

    // Play impact sound
    void PlayBoomSound()
    {
        if (boomSound == null) return;

        GameObject audioObj = new GameObject("TempAudio_Boom"); // Name it for easier identification
        audioObj.transform.position = transform.position;

        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = boomSound;
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        audioSource.Play();

        Destroy(audioObj, boomSound.length + 0.1f); // Slightly delay destruction
    }
}
