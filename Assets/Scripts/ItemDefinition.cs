using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Leo Game/Item")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string itemName = "Item";
    [SerializeField, TextArea(3, 6)] private string description = "";
    [SerializeField] private Sprite icon;
    [SerializeField] private bool stackable;

    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public bool Stackable => stackable;
}
