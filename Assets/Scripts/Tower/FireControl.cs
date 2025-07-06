using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireControl : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;       // Muzzle position

    public float fireRate = 1f;       // 1 shot per second
    public float bulletSpeed = 10f;
    [SerializeField] private float coldDelay = 0.5f;
    private float fireCountdown = 0f;
    public float bulletSpreadAngle = 5f;  // Bullet spread angle in degrees (e.g., 5 means б└5бу)

    private GameObject targetObject;

    public bool damageDirectly = false;
    [SerializeField] private int damage = 10;

    private EnemyTracking enemyTracking = null;

    public AudioClip fireSound;
    public float volume = 0.8f;

    private void Start()
    {
        enemyTracking = GetComponent<EnemyTracking>();
    }

    void Update()
    {
        if ((fireCountdown <= 0f) && enemyTracking.IsFire())
        {
            StartCoroutine(Shoot());
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;

        if (!enemyTracking.IsFire())
        {
            fireCountdown = coldDelay;
        }
    }

    IEnumerator Shoot()
    {
        foreach (Transform barrel in firePoint)  // Loop through all barrel (child) transforms
        {
            // Add random firing spread
            Quaternion spread = Quaternion.Euler(
                Random.Range(-bulletSpreadAngle, bulletSpreadAngle),
                Random.Range(-bulletSpreadAngle, bulletSpreadAngle),
                0f
            );

            Quaternion finalRotation = barrel.rotation * spread;

            GameObject bulletGO = Instantiate(bulletPrefab, barrel.position, finalRotation);
            Rigidbody rb = bulletGO.GetComponent<Rigidbody>();

            if (!damageDirectly)
            {
                IDamageable bullet = bulletGO.GetComponent<IDamageable>();
                bullet.SetDamage(damage);
            }

            rb.velocity = finalRotation * Vector3.forward * bulletSpeed;

            yield return new WaitForSeconds(0.002f);
        }

        if (damageDirectly)
        {
            targetObject = enemyTracking.targetObject;

            if (targetObject != null)
            {
                float distance = Vector3.Distance(transform.position, targetObject.transform.position);
                float flightTime = distance / bulletSpeed;

                StartCoroutine(DealDamage(targetObject.transform, flightTime));
            }
        }

        // Play firing sound with temporary AudioSource
        PlayFireSound();
    }

    private IEnumerator DealDamage(Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            if (target.CompareTag("Enemy"))
            {
                Debug.Log("Emotional Damaged!");
                SoldierBehavior health = targetObject.gameObject.GetComponent<SoldierBehavior>();
                health.TakeDamage(damage);
            }
            if (target.CompareTag("Tank"))
            {
                TankBossBehavior health = target.gameObject.GetComponent<TankBossBehavior>();
                health.TakeDamage(damage);
            }
        }
    }

    void PlayFireSound()
    {
        if (fireSound == null) return;

        // Create a temporary GameObject for audio
        GameObject audioObj = new GameObject("TempAudio");
        audioObj.transform.position = firePoint.position;

        // Add AudioSource component
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = fireSound;
        audioSource.spatialBlend = 0.5f; // 3D sound, if needed
        audioSource.volume = volume;
        audioSource.Play();

        // Destroy after sound has finished playing
        Destroy(audioObj, fireSound.length);
    }

    public void UpgradeDamage()
    {
        damage += damage / 5;
    }
}
