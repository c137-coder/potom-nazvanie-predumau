[System.Serializable]
public class InventoryEntry
{
    public ItemDefinition Item;
    public int Quantity;

    public InventoryEntry(ItemDefinition item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}
