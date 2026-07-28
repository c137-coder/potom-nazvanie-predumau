using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField] private float meleeRange = 0.6f;
    [SerializeField] private float meleeRadius = 0.5f;
    [SerializeField] private float meleeHeight = 0.6f;
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private float groundMeleeDuration = 0.4f;
    [SerializeField] private float airMeleeDuration = 0.35f;
    [SerializeField] private LayerMask enemyLayer = 1 << 9;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private int rangedDamage = 5;
    [SerializeField] private float rangedAttackDuration = 0.2f;
    [SerializeField] private float rangedCooldown = 0.7f;
    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private float reloadDuration = 1.5f;
    [SerializeField] private LayerMask groundLayer = 1 << 8;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip meleeHitClip;
    [SerializeField] private float meleeHitVolume = 2f;
    [SerializeField] private AudioClip meleeSwingClip;

    private PlayerMovement movement;

    private bool isAttacking;
    private float attackTimer;

    private float rangedCooldownTimer;
    private int currentAmmo;
    private bool isReloading;
    private float reloadTimer;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        currentAmmo = maxAmmo;
    }

    private void Start()
    {
        PublishAmmo();
    }

    private void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                movement.SetMovementLocked(false);
            }
        }

        if (rangedCooldownTimer > 0f)
        {
            rangedCooldownTimer -= Time.deltaTime;
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isReloading = false;
                currentAmmo = maxAmmo;
                PublishAmmo();

                if (animator != null)
                {
                    animator.SetBool("IsReloading", false);
                }
            }
        }
    }

    public void TryMeleeAttack()
    {
        if (isAttacking)
        {
            return;
        }

        bool isGrounded = movement.IsGrounded;
        isAttacking = true;
        attackTimer = isGrounded ? groundMeleeDuration : airMeleeDuration;
        movement.SetMovementLocked(isGrounded);

        if (animator != null)
        {
            animator.SetTrigger("MeleeAttack");
        }

        if (sfxSource != null && meleeSwingClip != null)
        {
            sfxSource.PlayOneShot(meleeSwingClip);
        }

        int facing = movement.FacingDirection;
        Vector2 origin = (Vector2)transform.position + new Vector2(facing * meleeRange, meleeHeight);
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, meleeRadius, enemyLayer);
        bool landedHit = false;
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(meleeDamage);
                landedHit = true;
            }
        }

        if (landedHit && sfxSource != null && meleeHitClip != null)
        {
            // Delayed slightly so it doesn't start on the exact same frame as the swing
            // whoosh and get masked by it — damage itself is still applied instantly above.
            Invoke(nameof(PlayMeleeHitSound), 0.08f);
        }
    }

    public void TryRangedAttack()
    {
        if (isAttacking || isReloading || rangedCooldownTimer > 0f || currentAmmo <= 0)
        {
            return;
        }

        isAttacking = true;
        attackTimer = rangedAttackDuration;
        rangedCooldownTimer = rangedCooldown;
        currentAmmo--;
        PublishAmmo();

        if (animator != null)
        {
            animator.SetTrigger("RangedAttack");
        }

        if (sfxSource != null && shootClip != null)
        {
            sfxSource.PlayOneShot(shootClip);
        }

        if (projectilePrefab != null)
        {
            int facing = movement.FacingDirection;
            Vector3 spawnPosition = transform.position + new Vector3(facing * 0.4f, meleeHeight, 0f);
            GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, projectilePrefab.transform.rotation);
            projectileObject.GetComponent<Projectile>().Launch(new Vector2(facing, 0f), projectileSpeed, rangedDamage, groundLayer, gameObject);
        }
    }

    public void TryReload()
    {
        if (isReloading || currentAmmo >= maxAmmo)
        {
            return;
        }

        isReloading = true;
        reloadTimer = reloadDuration;
        PublishAmmo();

        if (animator != null)
        {
            animator.SetBool("IsReloading", true);
        }

        if (sfxSource != null && reloadClip != null)
        {
            sfxSource.PlayOneShot(reloadClip);
        }
    }

    private void PlayMeleeHitSound()
    {
        sfxSource.PlayOneShot(meleeHitClip, meleeHitVolume);
    }

    private void PublishAmmo()
    {
        EventBus.Publish(new PlayerAmmoChanged(currentAmmo, maxAmmo, isReloading));
    }

    private void OnDrawGizmosSelected()
    {
        PlayerMovement movementRef = movement != null ? movement : GetComponent<PlayerMovement>();
        int facing = movementRef != null ? movementRef.FacingDirection : 1;

        Gizmos.color = Color.red;
        Vector3 meleeOrigin = transform.position + new Vector3(facing * meleeRange, meleeHeight, 0f);
        Gizmos.DrawWireSphere(meleeOrigin, meleeRadius);
    }
}
