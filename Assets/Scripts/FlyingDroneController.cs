using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class FlyingDroneController : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolReachThreshold = 0.15f;

    [Header("Engage")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float detectRadius = 6f;
    [SerializeField] private float loseSightRadius = 8f;
    [SerializeField] private float preferredRange = 4f;
    [SerializeField] private float rangeTolerance = 0.5f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask groundLayer = 1 << 8;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 1f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private Transform player;
    private Transform patrolTarget;

    private bool isChasing;
    private float attackCooldownTimer;
    private int facingDirection = 1;

    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private int dodgeSide = 1;

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
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
        dodgeSide = Random.value < 0.5f ? 1 : -1;
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
            Vector2 toPlayer = player != null ? (Vector2)(player.position - transform.position) : Vector2.right;
            if (toPlayer.sqrMagnitude < 0.001f)
            {
                toPlayer = Vector2.right;
            }
            Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x).normalized;
            rb.linearVelocity = perpendicular * dodgeSide * dodgeSpeed;
            UpdateVisuals();
            return;
        }

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        if (distanceToPlayer <= detectRadius || (isChasing && distanceToPlayer <= loseSightRadius))
        {
            isChasing = true;
            Engage(distanceToPlayer);
        }
        else
        {
            isChasing = false;
            Patrol();
        }

        UpdateVisuals();
    }

    private void Engage(float distanceToPlayer)
    {
        FacePlayer();

        Vector2 toPlayer = player.position - transform.position;
        Vector2 direction;
        if (distanceToPlayer > preferredRange + rangeTolerance)
        {
            direction = toPlayer.normalized;
        }
        else if (distanceToPlayer < preferredRange - rangeTolerance)
        {
            direction = -toPlayer.normalized;
        }
        else
        {
            direction = Vector2.zero;
        }

        rb.linearVelocity = direction * chaseSpeed;

        if (distanceToPlayer <= preferredRange + rangeTolerance && attackCooldownTimer <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        attackCooldownTimer = attackCooldown;

        if (projectilePrefab == null || player == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
        projectileObject.GetComponent<Projectile>().Launch(direction, projectileSpeed, attackDamage, groundLayer, gameObject);
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        facingDirection = player.position.x >= transform.position.x ? 1 : -1;
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, patrolTarget.position) <= patrolReachThreshold)
        {
            patrolTarget = patrolTarget == pointA ? pointB : pointA;
        }

        Vector2 direction = ((Vector2)patrolTarget.position - (Vector2)transform.position).normalized;
        facingDirection = direction.x >= 0f ? 1 : -1;
        rb.linearVelocity = direction * patrolSpeed;
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingDirection < 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, preferredRange);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
