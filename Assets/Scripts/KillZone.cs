using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    private const int LethalDamage = 9999;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(LethalDamage);
        }
    }
}
