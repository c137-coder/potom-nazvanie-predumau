using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions actions;
    private PlayerMovement movement;
    private PlayerCombat combat;
    private IInteractable nearbyInteractable;

    private void Awake()
    {
        actions = new InputSystem_Actions();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
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

    public void OnMove(InputAction.CallbackContext context)
    {
        movement.SetMoveInput(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        movement.RequestJump();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        movement.TryDash();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        combat.TryMeleeAttack();
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        combat.TryRangedAttack();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        combat.TryReload();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        PauseMenuController.Instance?.TogglePause();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        nearbyInteractable?.Interact();
    }

    public void SetNearbyInteractable(IInteractable interactable)
    {
        nearbyInteractable = interactable;
    }

    public void ClearNearbyInteractable(IInteractable interactable)
    {
        if (ReferenceEquals(nearbyInteractable, interactable))
        {
            nearbyInteractable = null;
        }
    }
}
