using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolReachThreshold = 0.1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float detectRadius = 4f;
    [SerializeField] private float loseSightRadius = 6f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackHitRadius = 0.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 6f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 1.2f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private Transform player;
    private Transform patrolTarget;

    private int facingDirection = 1;

    private bool isChasing;
    private bool isAttacking;
    private float attackTimer;
    private float attackCooldownTimer;

    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        patrolTarget = pointB != null ? pointB : pointA;
        health.Damaged += OnDamaged;
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.Damaged -= OnDamaged;
        }
    }

    private void OnDamaged()
    {
        if (dodgeCooldownTimer > 0f || health.IsDead)
        {
            return;
        }

        isDodging = true;
        isAttacking = false;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (health.IsDead)
        {
            return;
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (dodgeCooldownTimer > 0f)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0f)
            {
                isDodging = false;
            }
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (health.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isDodging)
        {
            rb.linearVelocity = new Vector2(-facingDirection * dodgeSpeed, rb.linearVelocity.y);
            UpdateVisuals();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            UpdateVisuals();
            return;
        }

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            FacePlayer();
            if (attackCooldownTimer <= 0f)
            {
                StartAttack();
            }
        }
        else if (distanceToPlayer <= detectRadius || (isChasing && distanceToPlayer <= loseSightRadius))
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }

        UpdateVisuals();
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        facingDirection = player.position.x >= transform.position.x ? 1 : -1;
    }

    private void ChasePlayer()
    {
        FacePlayer();
        rb.linearVelocity = new Vector2(facingDirection * chaseSpeed, rb.linearVelocity.y);
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (Vector2.Distance(transform.position, patrolTarget.position) <= patrolReachThreshold)
        {
            patrolTarget = patrolTarget == pointA ? pointB : pointA;
        }

        facingDirection = patrolTarget.position.x >= transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(facingDirection * patrolSpeed, rb.linearVelocity.y);
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown + attackDuration;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Vector2 origin = (Vector2)transform.position + new Vector2(facingDirection * attackRange, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackHitRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }

            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingDirection < 0;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        Gizmos.color = Color.red;
        Vector3 attackOrigin = transform.position + new Vector3(facingDirection * attackRange, 0f, 0f);
        Gizmos.DrawWireSphere(attackOrigin, attackHitRadius);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
