using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SavePoint : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject prompt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInputHandler inputHandler))
        {
            return;
        }

        inputHandler.SetNearbyInteractable(this);

        if (prompt != null)
        {
            prompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInputHandler inputHandler))
        {
            return;
        }

        inputHandler.ClearNearbyInteractable(this);

        if (prompt != null)
        {
            prompt.SetActive(false);
        }
    }

    public void Interact()
    {
        SaveSlotMenuController.Instance?.Open();
    }
}
