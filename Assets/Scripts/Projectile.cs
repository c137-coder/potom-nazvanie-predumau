using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private AudioClip hitClip;

    private int damage;
    private LayerMask groundLayer;
    private GameObject owner;

    public void Launch(Vector2 direction, float speed, int damageAmount, LayerMask ground, GameObject shooter)
    {
        damage = damageAmount;
        groundLayer = ground;
        owner = shooter;
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner)
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            PlayHitSound();
            Destroy(gameObject);
            return;
        }

        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            PlayHitSound();
            Destroy(gameObject);
        }
    }

    private void PlayHitSound()
    {
        if (hitClip != null)
        {
            AudioSource.PlayClipAtPoint(hitClip, transform.position);
        }
    }
}
