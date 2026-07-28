using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerInventory inventory))
        {
            return;
        }

        inventory.AddItem(item, amount);
        Destroy(gameObject);
    }
}
