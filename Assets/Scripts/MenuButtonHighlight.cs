using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject frame;

    public void OnSelect(BaseEventData eventData)
    {
        if (frame != null)
        {
            frame.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (frame != null)
        {
            frame.SetActive(false);
        }
    }
}
