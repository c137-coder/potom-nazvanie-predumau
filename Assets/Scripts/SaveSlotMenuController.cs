using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenuController : MonoBehaviour
{
    public static SaveSlotMenuController Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private Text[] slotLabels;
    [SerializeField] private PlayerMovement playerMovement;

    private bool wasPausedBeforeOpen;

    private void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        RefreshLabels();
        panel.SetActive(true);
        Time.timeScale = 0f;

        if (EventSystem.current != null && firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void SaveToSlot(int slot)
    {
        SaveSystem.Save(slot, SceneManager.GetActiveScene().name, playerMovement.transform.position);
        Close();
    }

    public void Cancel()
    {
        Close();
    }

    private void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        if (playerMovement != null)
        {
            playerMovement.ClearPendingInput();
        }
    }

    private void RefreshLabels()
    {
        for (int i = 0; i < slotLabels.Length; i++)
        {
            int slot = i + 1;
            string status = SaveSystem.HasSave(slot) ? "занят" : "пусто";
            slotLabels[i].text = $"Слот {slot} ({status})";
        }
    }
}
