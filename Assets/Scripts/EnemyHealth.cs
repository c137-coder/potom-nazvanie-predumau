using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float deathDuration = 0.6f;
    [SerializeField] private Animator animator;

    private int currentHealth;
    private bool isDead;

    public bool IsDead => isDead;

    // Same-GameObject notification (not EventBus — only siblings on this enemy care, see ARCHITECTURE.md 3.5).
    public event Action Damaged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }

            Damaged?.Invoke();
        }
    }

    private void Die()
    {
        isDead = true;

        if (TryGetComponent(out Collider2D col))
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Destroy(gameObject, deathDuration);
    }
}
