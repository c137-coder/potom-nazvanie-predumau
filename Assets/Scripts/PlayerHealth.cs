using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        PublishHealth();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        PublishHealth();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void PublishHealth()
    {
        EventBus.Publish(new PlayerHealthChanged(currentHealth, maxHealth));
    }

    private void Die()
    {
        isDead = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
