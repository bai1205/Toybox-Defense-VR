using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerBullet : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the bullet after 3 seconds if it doesn't hit anything
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Hit enemy logic, apply damage
            Destroy(gameObject);
            Debug.Log("Emotional Damaged!");
            TowerBehavior health = other.gameObject.GetComponent<TowerBehavior>();
            health.TakeDamage(damage);
        }

        if (other.CompareTag("Tank"))
        {
            // Hit tank logic, apply damage
            Destroy(gameObject);
            Debug.Log("Emotional Damaged!");
            TankBossBehavior health = other.gameObject.GetComponent<TankBossBehavior>();
            health.TakeDamage(damage);
        }
    }
}
