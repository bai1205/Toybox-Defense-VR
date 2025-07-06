using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MachineGunBullet : MonoBehaviour, IDamageable
{
    public float lifetime = 3f;
    public int damage = 10;

    public AudioClip boomSound;
    public float volume = 0.8f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the bullet after 3 seconds if it doesn't hit anything
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Hit enemy logic, apply damage
            Debug.Log("Emotional Damaged!");
            SoldierBehavior health1 = other.gameObject.GetComponent<SoldierBehavior>();
            health1.TakeDamage(damage);
        }

        if (other.CompareTag("Tank"))
        {
            // Hit tank logic, apply reduced damage
            Debug.Log("Emotional Damaged!");
            TankBossBehavior health = other.gameObject.GetComponent<TankBossBehavior>();
            health.TakeDamage(damage / 10);
        }

        Destroy(gameObject);
        PlayBoomSound();
    }

    void PlayBoomSound()
    {
        if (boomSound == null) return;

        // Create a temporary GameObject for the sound
        GameObject audioObj = new GameObject("TempAudio");
        audioObj.transform.position = transform.position; // Place the sound at the impact position

        // Add AudioSource component
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = boomSound;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.volume = volume;
        audioSource.Play();

        // Destroy the audio object after the sound finishes playing
        Destroy(audioObj, boomSound.length);
    }
}
