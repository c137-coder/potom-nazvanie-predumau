using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadSlotMenuController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject mainMenuButtonsRoot;
    [SerializeField] private Text[] slotLabels;
    [SerializeField] private Button[] slotButtons;

    public void Open()
    {
        for (int i = 0; i < slotLabels.Length; i++)
        {
            int slot = i + 1;
            bool hasSave = SaveSystem.HasSave(slot);
            slotLabels[i].text = hasSave ? $"Слот {slot}" : $"Слот {slot} (пусто)";
            slotButtons[i].interactable = hasSave;
        }

        if (mainMenuButtonsRoot != null)
        {
            mainMenuButtonsRoot.SetActive(false);
        }

        panel.SetActive(true);

        if (EventSystem.current != null && firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void LoadSlot(int slot)
    {
        SaveSystem.LoadIntoScene(slot);
    }

    public void Cancel()
    {
        panel.SetActive(false);

        if (mainMenuButtonsRoot != null)
        {
            mainMenuButtonsRoot.SetActive(true);
        }
    }
}
