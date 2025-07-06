using UnityEngine;
using UnityEngine.InputSystem; // Reference to the new Input System namespace
using System.Collections;

public class PlayerLaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    public int laserDamage = 20;         // Laser damage (default 5)
    public float fireCooldown = 5f;      // Firing cooldown (default 5 seconds)
    public float bulletSpeed = 50f;      // Adjustable speed

    [Header("Component and Prefab References")]
    public Transform laserFirePoint;         // Laser fire point (assign the muzzle GameObject)
    public GameObject laserProjectilePrefab; // Laser projectile prefab
    public InputActionProperty fireAction;   // Fire input action (link to Input Action Asset)

    private float nextFireTime = 0f;         // Used for cooldown timing

    // --- Input event handling ---
    private void OnEnable()
    {
        // Check if Fire Action is properly set
        if (fireAction == null || fireAction.action == null)
        {
            Debug.LogError("Fire Action not set correctly in Inspector!", this);
            // return; // Don¡¯t return here to allow keyboard testing
        }

        // Always attempt to enable the action (if reference exists)
        if (fireAction != null && fireAction.action != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed += OnFireButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (fireAction != null && fireAction.action != null)
        {
            // Unsubscribe callback and disable the action
            fireAction.action.performed -= OnFireButtonPressed;
            fireAction.action.Disable();
        }
    }

    // Called when the input action (e.g., grip button) is pressed
    private void OnFireButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("VR Grip input triggered!"); // Log for confirming VR input
        TryFireLaser();
    }
    // --- End of input event handling ---

    // --- Add Update for keyboard testing ---
    void Update()
    {
#if UNITY_EDITOR
        // Check for space key press for testing without VR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key input triggered! (Editor-only test)");
            TryFireLaser();
        }
#endif
    }
    // --- End of Update ---

    // Attempt to fire the laser (check cooldown)
    void TryFireLaser()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            FireLaser();
        }
        else
        {
            Debug.Log("Laser is cooling down..."); // Optional: add sound or visual feedback
        }
    }

    // Perform the laser firing action
    void FireLaser()
    {
        if (laserFirePoint == null) { Debug.LogError("Laser Fire Point is not assigned!", this); return; }
        if (laserProjectilePrefab == null) { Debug.LogError("Laser Projectile Prefab is not assigned!", this); return; }

        Debug.Log("Firing laser! (FireLaser)");

        // --- Visual effects + shooting logic ---
        // Instantiate the laser projectile at the fire point (with physics)
        GameObject projectileGO = Instantiate(laserProjectilePrefab, laserFirePoint.position, laserFirePoint.rotation);

        // --- Set damage (optional but recommended) ---
        // Get the bullet script and set the damage value
        MachineGunBullet bulletScript = projectileGO.GetComponent<MachineGunBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(laserDamage); // Use the damage value defined here
        }
        else
        {
            Debug.LogWarning($"MachineGunBullet script not found on prefab '{projectileGO.name}'!", projectileGO);
        }

        // --- Set velocity ---
        // Get Rigidbody and assign velocity
        Rigidbody rb = projectileGO.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = laserFirePoint.forward * bulletSpeed;
        }
        else
        {
            Debug.LogWarning($"Laser projectile prefab '{laserProjectilePrefab.name}' does not have a Rigidbody component. Cannot apply velocity. Please add one.", laserProjectilePrefab);
        }
    }
}
