using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]

public class TankBossBehavior : HealthBase
{
    [Header("Track Animation")]
    public Animator trackAnimator;
    private Vector3 _lastPosition; // Used to calculate speed

    [Header("Movement Settings")]
    public float moveSpeed = 1.2f;
    public float rotateSpeed = 0.8f;

    [Header("Attack Settings")]
    public float detectionRadius = 8f;
    public int attackDamage = 30;
    public float attackCooldown = 5f;

    [Header("Attack Effects")]
    public ParticleSystem muzzleFlash;

    [Header("Turret Control")]
    public Transform turretRoot;  // Root of the turret
    public float turretRotateSpeed = 3f;

    [Header("UI")]
    public Image healthFillImage;           // Optional: drag in a health bar Slider (if any)
    public Transform pentagramUI;           // Transform of the pentagram model
    public float pentagramRotateSpeed = 45f; // Rotation speed (degrees/second)

    [Header("Death Effects")]
    public GameObject explosionPrefab; // Drag in the explosion effect prefab
    public Rigidbody turretRigidbody;  // Drag in the Rigidbody of the turret
    public float popUpForce = 10f;     // Upward force applied to turret
    public float destroyDelay = 2f;    // Delay before destruction after death
    private const string deadTag = "DeadEnemy"; // Using a constant is safer

    [Header("AudioClips")]
    public AudioClip moveLoopClip;     // Movement loop sound
    public AudioClip fireClip;         // Fire sound
    public AudioClip damageClip;       // Damage sound
    public AudioClip deathClip;        // Death sound
    public AudioSource audioSource;
    public AudioSource damageSource;

    private Transform[] waypoints;
    private int currentIndex = 0;
    private TowerBehavior currentTarget;
    private bool isAttacking;
    private bool isInCooldown;
    private bool isDead = false;   // Flag to indicate if already dead
    private bool isMoving = false; // Flag to indicate if moving


    protected override void Start()
    {
        base.Start(); // Initialize health
        _lastPosition = transform.position; // Initialize last position

        // Initialize health bar logic
        if (maxHealth <= 0) maxHealth = 100;
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = 1f; // Full health
        }

        InitializePath();
        StartCoroutine(DetectionRoutine());

        if (trackAnimator == null)
        {
            Transform tankArmature = transform.Find("TankArmature");
            if (tankArmature != null)
            {
                trackAnimator = tankArmature.GetComponent<Animator>();
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found! Cannot play sounds.", this);
        }
    }

    void Update()
    {
        RotatePentagramUI();

        if (isDead) return;

        if (!isAttacking)
        {
            MoveAlongPath();
        }
        UpdateTrackAnimation();
    }

    void InitializePath()
    {
        // Reuse original path initialization logic
        if (GameObject.Find("PathHolder")?.TryGetComponent<PathFollower>(out var pathProvider) ?? false)
        {
            waypoints = pathProvider.GetPath();
        }
        else
        {
            Debug.LogError("Path initialization failed!");
            enabled = false;
        }
    }

    IEnumerator DetectionRoutine()
    {
        while (true)
        {
            if (!isAttacking && !isInCooldown && (currentTarget == null || currentTarget.IsDead())) // when tower is destroyed
            {
                FindNearestTower();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void FindNearestTower()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float minDistance = float.MaxValue;
        TowerBehavior nearestTower = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower") && hit.TryGetComponent<TowerBehavior>(out var tower))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTower = tower;
                }
            }
        }

        if (nearestTower != null)
        {
            StartCoroutine(AttackRoutine(nearestTower));
        }
    }

    void MoveAlongPath()
    {
        // Prevent movement if there's a valid living target
        bool hasValidTarget = currentTarget != null && !currentTarget.IsDead();
        if (currentIndex >= waypoints.Length || isAttacking || hasValidTarget)
        {
            isMoving = false;
            return;
        }

        Transform target = waypoints[currentIndex];
        Vector3 targetPosition = target.position;

        // Move the base
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Rotate base (Y axis only)
        RotateBaseTowards(target.position);

        // Update waypoint index
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                isMoving = false; // Reached last point
                // Optional: stop or trigger other logic
            }
        }
    }

    void RotateBaseTowards(Vector3 targetPosition)
    {
        Vector3 flatDirection = new Vector3(
            targetPosition.x - transform.position.x,
            0,
            targetPosition.z - transform.position.z
        ).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    IEnumerator AttackRoutine(TowerBehavior target)
    {
        Debug.Log($"Starting attack on target: {target.name}, Position: {target.transform.position}");
        Vector3 attackPosition = transform.position;
        isAttacking = true;
        isMoving = false; // Stop moving
        int originalIndex = currentIndex;
        currentIndex = waypoints.Length; // Stop moving
        currentTarget = target;

        while (currentTarget != null)
        {
            // Turret rotates independently toward target
            Vector3 targetDir = target.transform.position - turretRoot.position;
            targetDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            // Smooth turret rotation
            float rotateProgress = 0;
            while (rotateProgress < 1)
            {
                turretRoot.rotation = Quaternion.Slerp(
                    turretRoot.rotation,
                    targetRotation,
                    rotateProgress
                );
                rotateProgress += Time.deltaTime * turretRotateSpeed;
                yield return null;
            }

            // Play fire sound
            if (audioSource != null && fireClip != null)
            {
                audioSource.loop = false; // Stop looping
                Debug.Log("Playing fire sound");
                audioSource.PlayOneShot(fireClip);
                Debug.Log("!!! [AttackRoutine] Fire sound PlayOneShot called.");
            }

            // Trigger muzzle flash and damage
            Debug.Log("Firing cannon!");
            if (muzzleFlash != null)
            {
                muzzleFlash.transform.rotation = turretRoot.rotation;
                muzzleFlash.Play();
                Debug.Log("Muzzle flash triggered");
                Debug.Log($"Effect position: {muzzleFlash.transform.position}");
            }
            target.TakeDamage(attackDamage);
            Debug.Log($"Dealt {attackDamage} damage to {target.name}");

            // Cooldown
            yield return new WaitForSeconds(attackCooldown);
        }

        // Wait until the target is destroyed
        while (!target.IsDead())
        {
            yield return null;
        }

        Debug.Log("Target destroyed or invalid, resuming movement");
        currentTarget = null;
        isInCooldown = false;
        isAttacking = false;
        currentIndex = originalIndex; // Resume path
    }

    void UpdateTrackAnimation()
    {
        if (trackAnimator == null) return;

        // Force speed to zero if attacking
        float speed = isAttacking ? 0 : (transform.position - _lastPosition).magnitude / Time.deltaTime;

        _lastPosition = transform.position;

        // Smooth speed value
        float smoothSpeed = Mathf.Lerp(trackAnimator.GetFloat("MoveSpeed"), speed, 10f * Time.deltaTime);
        bool isMoving = smoothSpeed > 0.05f;

        trackAnimator.SetFloat("MoveSpeed", smoothSpeed);
        trackAnimator.SetBool("IsMoving", isMoving);

        if (damageSource != null && moveLoopClip != null)
        {
            if (isMoving && !damageSource.isPlaying)
            {
                damageSource.clip = moveLoopClip;
                damageSource.loop = true;
                damageSource.Play();
            }
            else if (!isMoving && damageSource.isPlaying && damageSource.clip == moveLoopClip)
            {
                Debug.Log($"!!! [UpdateTrackAnimation] Preparing to stop move sound (Current Clip: {damageSource.clip?.name})");
                damageSource.Stop();
                damageSource.loop = false;
            }
        }
    }

    /// <summary>
    /// Take damage from towers
    /// </summary>
    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        if (damageClip != null && audioSource != null)
        {
            damageSource.PlayOneShot(damageClip);
        }

        base.TakeDamage(damage);

        if (healthFillImage != null && maxHealth > 0)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Rotate the pentagram UI continuously
    /// </summary>
    void RotatePentagramUI()
    {
        if (pentagramUI == null)
        {
            Debug.LogWarning("Pentagram UI not assigned!");
            return;
        }
        pentagramUI.Rotate(Vector3.up, pentagramRotateSpeed * Time.deltaTime, Space.Self);
    }

    protected override void Die()
    {
        Debug.Log("Die() method has been called!");
        if (isDead) return;
        isDead = true;
        isMoving = false;

        Debug.Log($"!!! [Die] Preparing to stop moveAttackDeathSource.");
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (damageClip != null && audioSource != null)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(deathClip);
        }

        Debug.Log($"Tank Boss {name} destroyed!");

        CurrencyManager.Instance.AddMoney(100);
        Debug.Log("now money: " + CurrencyManager.Instance.currentMoney);
        GameManager.Instance.EnemyKilled();
        this.enabled = false;

        SetTagRecursively(gameObject.transform, deadTag);
        Debug.Log($"{gameObject.name} and its children tags changed to {deadTag}");

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Explosion effect spawned");
        }
        else
        {
            Debug.LogWarning("Explosion prefab not assigned!");
        }

        if (turretRigidbody != null)
        {
            turretRigidbody.transform.SetParent(null, true);
            SetTagRecursively(turretRigidbody.transform, deadTag);

            turretRigidbody.isKinematic = false;
            turretRigidbody.useGravity = true;

            turretRigidbody.AddForce(Vector3.up * popUpForce, ForceMode.Impulse);
            turretRigidbody.AddTorque(Random.insideUnitSphere * popUpForce * 0.5f, ForceMode.Impulse);

            Debug.Log("Turret popped up!");
        }
        else
        {
            Debug.LogWarning("Turret Rigidbody not assigned!");
        }

        Destroy(gameObject, destroyDelay);
        Debug.Log($"Tank will be destroyed in {destroyDelay} seconds");

        if (turretRigidbody != null)
        {
            Destroy(turretRigidbody.gameObject, destroyDelay);
        }
    }

    // Recursively set the tag of a GameObject and all its children
    void SetTagRecursively(Transform parent, string tag)
    {
        if (parent == null) return;

        parent.gameObject.tag = tag;

        foreach (Transform child in parent)
        {
            SetTagRecursively(child, tag);
        }
    }
}
