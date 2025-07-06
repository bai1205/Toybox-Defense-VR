using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerBehavior : HealthBase
{
    [Header("Health Bar")]
    public Image healthFillImage;

    protected override void Start()
    {
        base.Start(); // Initialize health

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = 1f; // Full health
        }
    }

    /// <summary>
    /// Tower takes damage
    /// </summary>
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Tower death logic
    /// </summary>
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        FindButtonController.FindButtonAndSetBuildFalse(gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// Check if tower is dead
    /// </summary>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
