using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private LoadSlotMenuController loadSlotMenu;

    private InputSystem_Actions actions;

    private void Awake()
    {
        actions = new InputSystem_Actions();

        if (continueButton != null)
        {
            continueButton.SetActive(SaveSystem.HasAnySave());
        }

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

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        loadSlotMenu.Open();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
