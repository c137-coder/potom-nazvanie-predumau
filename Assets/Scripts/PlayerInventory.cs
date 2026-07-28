using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> startingItems = new();

    private readonly List<InventoryEntry> entries = new();

    public IReadOnlyList<InventoryEntry> Entries => entries;

    private void Awake()
    {
        foreach (ItemDefinition item in startingItems)
        {
            AddItem(item, 1);
        }
    }

    public void AddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return;
        }

        if (item.Stackable)
        {
            InventoryEntry existing = entries.Find(e => e.Item == item);
            if (existing != null)
            {
                existing.Quantity += amount;
                EventBus.Publish(new InventoryChanged());
                return;
            }
        }

        entries.Add(new InventoryEntry(item, amount));
        EventBus.Publish(new InventoryChanged());
    }
}
