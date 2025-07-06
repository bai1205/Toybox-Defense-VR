using UnityEngine;
/// <summary>
/// The health base class for all characters
/// </summary>
public abstract class HealthBase : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Debug.Log($"{gameObject.name} 受到 {damage} 点伤害，剩余血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die(); 
}
