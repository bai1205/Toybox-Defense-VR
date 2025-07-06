using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optimized soldier behavior script, including movement, attack, and health management functions
/// Inherit from the HealthBase class to handle basic health logic
/// </summary>
public class SoldierBehavior : HealthBase
{
    // Path points and movement control
    private Transform[] waypoints;    // Store an array of moving path points
    private int currentIndex = 0;
    private Animator animator;

    [Header("Move Setting")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    [Header("Attack Settings")]
    public float detectionRadius = 5f; // Detect the radius of the enemy
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public int Money = 40;

    [Header("Effects Settings")]
    public GameObject muzzleFlashPrefab;  // Gun muzzle spark effect prefabricated body
    public Transform muzzlePoint;

    [Header("Audio Setting")]
    public AudioClip shootSFX;
    private AudioSource audioSource;
    [Header("HealthBar")]
    public Image healthFillImage;

    private bool canMove = true;
    private bool isAttacking = false;
    private bool isInCooldown = false;
    private TowerBehavior currentTarget;
    private bool alreadyDead = false;
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        InitializePath();

        if (muzzlePoint == null)
        {
            muzzlePoint = FindChildRecursive(transform, "MuzzlePoint");
            if (muzzlePoint != null)
                Debug.Log("MuzzlePoint 自动绑定成功！");
            else
                Debug.LogWarning("MuzzlePoint 查找失败，请检查名称或结构！");
        }
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = 1f; // 满血
        }

        StartCoroutine(DetectionRoutine());
    }

    /// <summary>
    /// Initialize the movement path
    /// Retrieve path points from PathHolder objects in the scene
    /// </summary>
    void InitializePath()
    {

        if (GameObject.Find("PathHolder")?.TryGetComponent<PathFollower>(out var pathProvider) ?? false)
        {
            waypoints = pathProvider.GetPath();
        }
        else
        {
            Debug.LogError("路径初始化失败！");
            enabled = false;
        }
    }

    /// <summary>
    /// Enemy Detection Protocol
    /// Detect towers within the range every 0.2 seconds
    /// </summary>
    IEnumerator DetectionRoutine()
    {
        while (true)
        {

            if (!isAttacking && !isInCooldown)
            {
                FindNearestTower();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update()
    {
        // Allow movement and execute movement logic when not in an attack state
        if (canMove && !isAttacking)
        {
            MoveAlongPath();
        }
    }

    #region Move System
    /// <summary>
    /// The main logic of moving along path points
    /// </summary>
    // Increase the small deviation of soldiers when walking
    private Vector3 offsetTargetPos = Vector3.zero;
    private bool offsetGenerated = false;

    void MoveAlongPath()
    {
        if (currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];

        if (!offsetGenerated)
        {

            offsetTargetPos = target.position + GetRandomOffset();
            offsetGenerated = true;
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            offsetTargetPos,
            moveSpeed * Time.deltaTime
        );

        RotateTowards(offsetTargetPos);

        animator.SetTrigger("run");

        // Upon reaching the vicinity of the offset target, proceed to the next point
        if (Vector3.Distance(transform.position, offsetTargetPos) < 0.2f)
        {
            if (currentIndex == waypoints.Length - 1)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                currentIndex++;
                offsetGenerated = false;
            }
        }
    }

    /// <summary>
    /// Generate random offset
    /// </summary>
    /// <returns>Return a Vector3 with a small random offset</returns>
    Vector3 GetRandomOffset()
    {
        float offsetRange = 1.5f; // Offset range 
        Vector2 randomCircle = Random.insideUnitCircle * offsetRange;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }


    /// <summary>
    /// Smooth steering towards the target position
    /// Rotate only around the Y-axis and keep the character upright
    /// </summary>
    void RotateTowards(Vector3 targetPosition)
    {
        // Calculate the orientation direction and ignore the Y-axis
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smooth rotation using spherical interpolation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            rotateSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Attack System
    /// <summary>
    /// Immediately turn to the target position
    /// </summary>
    void RotateImmediately(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; //Maintain horizontal rotation
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Find the nearest tower as an attack target
    /// </summary>
    void FindNearestTower()
    {
        // All collision bodies within the spherical detection range
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float minDistance = float.MaxValue;
        TowerBehavior nearestTower = null;

        foreach (var hit in hits)
        {
            // Check if it is a tower object
            if (hit.CompareTag("Tower") && hit.TryGetComponent<TowerBehavior>(out var tower))
            {
                // Calculate distance and find the nearest one
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTower = tower;
                }
            }
        }

        // Once a valid target is found, the attack begins
        if (nearestTower != null)
        {
            StartCoroutine(AttackRoutine(nearestTower));
        }
    }

    /// <summary>
    /// Attack process coroutine
    /// Complete sequence including turn, attack animation, damage calculation, and cooldown
    /// </summary>
    IEnumerator AttackRoutine(TowerBehavior target)
    {
        // 1.Enter attack state
        isAttacking = true;
        canMove = false; // 禁止移动
        currentTarget = target;

        // 2. Immediately turn towards the target
        RotateImmediately(target.transform.position);

        // 3. Play attack animation
        animator.SetTrigger("shoot");

        //  Play muzzle spark effect
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 1f); // Automatically destroy special effects object
        }
        if (shootSFX != null && muzzlePoint != null)
        {
            audioSource.PlayOneShot(shootSFX);
        }

        // 4. 攻击Shake and wait before attacking (0.3 seconds)前摇等待(0.3秒)
        yield return new WaitForSeconds(0.3f);

        // 5. Actual damage caused (check if the target still exists)
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(attackDamage);
        }

        // 6. Shake and wait after attack (0.7 seconds)
        yield return new WaitForSeconds(0.7f);

        // 7. Entering the cooling phase
        canMove = true;     // Restore Mobile
        isAttacking = false; // End attack state
        isInCooldown = true; // Start cooling

        // 8.Cooling timer (total cooling time=front shake+back shake+cooling)
        yield return new WaitForSeconds(attackCooldown);
        isInCooldown = false;
    }

    #endregion

    /// <summary>
    /// Death handling (inherited from HealthBase)
    /// </summary>
    protected override void Die()
    {
        if (!alreadyDead)
        {
            alreadyDead = true;
            StopAllCoroutines();
            Debug.Log($"{name} 被击败");
            Destroy(gameObject);
            CurrencyManager.Instance.AddMoney(Money);
            GameManager.Instance.EnemyKilled();
            Debug.Log("now money: " + CurrencyManager.Instance.currentMoney);
        }
    }
    /// <summary>
    /// Soldier Injury Logic
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Blood deduction
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
    /// <summary>
    /// Recursively search for objects with specific names among all sub objects
    /// </summary>
    Transform FindChildRecursive(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
                return child;

            Transform result = FindChildRecursive(child, targetName);
            if (result != null)
                return result;
        }
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            GameManager.Instance.EnemyReachedBase();
            //Destroy(gameObject);
        }
    }
}
