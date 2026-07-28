using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject returnFocusTarget;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private RectTransform itemListContainer;
    [SerializeField] private GameObject itemButtonTemplate;
    [SerializeField] private Text descriptionText;

    private readonly List<GameObject> spawnedButtons = new();

    private void OnEnable()
    {
        EventBus.Subscribe<InventoryChanged>(OnInventoryChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<InventoryChanged>(OnInventoryChanged);
    }

    private void OnInventoryChanged(InventoryChanged e)
    {
        if (panel.activeSelf)
        {
            RefreshList();
        }
    }

    public void Open()
    {
        // Deactivating pausePanel while one of its buttons (e.g. this one) is still the
        // selected UI element skips OnDeselect, leaving its MenuButtonHighlight frame stuck
        // visible. Clear selection first so the deselect callback fires while it's still active.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        pausePanel.SetActive(false);
        panel.SetActive(true);
        RefreshList();
    }

    public void Close()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        panel.SetActive(false);
        pausePanel.SetActive(true);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(returnFocusTarget);
        }
    }

    public void ForceClose()
    {
        if (panel.activeSelf && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        panel.SetActive(false);
    }

    private void RefreshList()
    {
        foreach (GameObject go in spawnedButtons)
        {
            Destroy(go);
        }
        spawnedButtons.Clear();

        IReadOnlyList<InventoryEntry> entries = playerInventory.Entries;
        GameObject firstButton = null;

        for (int i = 0; i < entries.Count; i++)
        {
            InventoryEntry entry = entries[i];
            GameObject buttonObject = Instantiate(itemButtonTemplate, itemListContainer);
            buttonObject.SetActive(true);
            buttonObject.name = $"ItemButton_{i}";

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0f, -i * 60f);

            string quantitySuffix = entry.Item.Stackable && entry.Quantity > 1 ? $" x{entry.Quantity}" : "";
            Text label = buttonObject.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = entry.Item.ItemName + quantitySuffix;
            }

            ItemDefinition item = entry.Item;
            Button button = buttonObject.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => ShowDescription(item));

            spawnedButtons.Add(buttonObject);
            if (firstButton == null)
            {
                firstButton = button.gameObject;
            }
        }

        if (entries.Count > 0)
        {
            ShowDescription(entries[0].Item);
        }
        else if (descriptionText != null)
        {
            descriptionText.text = "Инвентарь пуст.";
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton != null ? firstButton : returnFocusTarget);
        }
    }

    private void ShowDescription(ItemDefinition item)
    {
        if (descriptionText == null || item == null)
        {
            return;
        }

        string text = $"{item.ItemName}\n\n{item.Description}";
        if (item is WeaponDefinition weapon)
        {
            string typeLabel = weapon.WeaponType == WeaponType.Melee ? "Ближний бой" : "Дальний бой";
            text += $"\n\nТип: {typeLabel}\nУрон: {weapon.Damage}";
        }

        descriptionText.text = text;
    }
}
