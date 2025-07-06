using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    public float horizontalSpeed = 30f;  // Horizontal rotation speed (degrees/second)
    public float verticalSpeed = 20f;    // Elevation rotation speed (degrees/second)
    public float maxElevation = 15f;     // Maximum upward angle
    public float maxDepression = 5f;     // Maximum downward angle

    private Transform gun;               // Gun barrel object
    private Vector3 targetDirection;     // Target direction

    void Start()
    {
        gun = transform.Find("Gun"); // Get the gun barrel by name from hierarchy
    }

    void Update()
    {
        // Example: aim at mouse position (can be replaced with actual target position)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetDirection = (hit.point - transform.position).normalized;
        }

        RotateTurret();
    }

    void RotateTurret()
    {
        // Turret horizontal rotation (Y axis)
        Quaternion targetHorizontal = Quaternion.LookRotation(
            new Vector3(targetDirection.x, 0, targetDirection.z)
        );
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetHorizontal,
            horizontalSpeed * Time.deltaTime
        );

        // Gun barrel pitch rotation (X axis)
        float targetAngle = Mathf.Clamp(
            Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.right),
            -maxDepression,
            maxElevation
        );
        gun.localRotation = Quaternion.Euler(
            targetAngle,
            gun.localEulerAngles.y,
            gun.localEulerAngles.z
        );
    }
}
