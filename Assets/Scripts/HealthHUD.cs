using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fullColor = new Color(0.3f, 0.85f, 0.3f);
    [SerializeField] private Color emptyColor = new Color(0.85f, 0.2f, 0.2f);

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHealthChanged>(OnHealthChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHealthChanged>(OnHealthChanged);
    }

    private void OnHealthChanged(PlayerHealthChanged e)
    {
        if (healthText != null)
        {
            healthText.text = $"{e.Current}/{e.Max}";
        }

        if (fillImage != null)
        {
            float ratio = e.Max > 0 ? (float)e.Current / e.Max : 0f;
            fillImage.fillAmount = ratio;
            fillImage.color = Color.Lerp(emptyColor, fullColor, ratio);
        }
    }
}
