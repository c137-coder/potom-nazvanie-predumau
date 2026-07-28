using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InventoryMenuController inventoryMenu;
    [SerializeField] private string menuSceneName = "MainMenu";

    private InputSystem_Actions actions;
    private bool isPaused;

    private void Awake()
    {
        Instance = this;
        actions = new InputSystem_Actions();

        InputSystemUIInputModule uiModule = FindFirstObjectByType<InputSystemUIInputModule>();
        if (uiModule != null)
        {
            uiModule.point = InputActionReference.Create(actions.UI.Point);
            uiModule.leftClick = InputActionReference.Create(actions.UI.Click);
            uiModule.rightClick = InputActionReference.Create(actions.UI.RightClick);
            uiModule.middleClick = InputActionReference.Create(actions.UI.MiddleClick);
            uiModule.scrollWheel = InputActionReference.Create(actions.UI.ScrollWheel);
            uiModule.move = InputActionReference.Create(actions.UI.Navigate);
            uiModule.submit = InputActionReference.Create(actions.UI.Submit);
            uiModule.cancel = InputActionReference.Create(actions.UI.Cancel);
        }
    }

    private void OnEnable()
    {
        actions.UI.Enable();
    }

    private void OnDisable()
    {
        actions.UI.Disable();
    }

    private void OnDestroy()
    {
        actions.Dispose();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        if (EventSystem.current != null && firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Clear selection before hiding the panel — deactivating a GameObject while it's
        // still the selected UI element skips its OnDeselect callback, leaving MenuButtonHighlight's
        // frame stuck visible next time the panel reappears.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        pausePanel.SetActive(false);
        inventoryMenu?.ForceClose();

        if (playerMovement != null)
        {
            playerMovement.ClearPendingInput();
        }
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
