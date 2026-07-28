using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int maxJumps = 2;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private int maxDashes = 1;
    [SerializeField] private float dashCooldown = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private InputSystem_Actions actions;
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private int facingDirection = 1;
    private int jumpsRemaining;
    private bool isGrounded;
    private bool jumpRequested;

    private bool isDashing;
    private float dashTimer;
    private int dashesRemaining;
    private float dashCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        actions = new InputSystem_Actions();
    }

    private void Start()
    {
        Vector2? spawnPosition = SaveSystem.ConsumePendingSpawnPosition();
        if (spawnPosition.HasValue)
        {
            transform.position = new Vector3(spawnPosition.Value.x, spawnPosition.Value.y, transform.position.z);
        }
    }

    private void OnEnable()
    {
        actions.Player.SetCallbacks(this);
        actions.Player.Enable();
    }

    private void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.RemoveCallbacks(this);
    }

    private void Update()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        // Ground check and jump consumption happen in the same physics step so a jump
        // triggered this frame can't be immediately refilled by a stale grounded reading
        // (the position only moves once this FixedUpdate's physics step is integrated).
        isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            dashesRemaining = maxDashes;
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            if (jumpsRemaining > 0)
            {
                jumpsRemaining--;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        if (isDashing)
        {
            UpdateVisuals();
            return;
        }

        if (moveInput.x != 0f)
        {
            facingDirection = moveInput.x > 0f ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingDirection < 0;
        }

        if (animator == null)
        {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsDashing", isDashing);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashesRemaining--;
        dashCooldownTimer = dashCooldown;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        jumpRequested = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed || isDashing || dashesRemaining <= 0 || dashCooldownTimer > 0f)
        {
            return;
        }

        StartDash();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // TODO: ближний бой — подключить, когда появится боевая система
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // TODO: дальний бой — подключить, когда появится боевая система
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        PauseMenuController.Instance?.TogglePause();
    }

    public void ClearPendingInput()
    {
        moveInput = Vector2.zero;
        jumpRequested = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
