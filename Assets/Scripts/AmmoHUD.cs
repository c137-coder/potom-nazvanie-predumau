using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] private Text ammoText;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerAmmoChanged>(OnAmmoChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerAmmoChanged>(OnAmmoChanged);
    }

    private void OnAmmoChanged(PlayerAmmoChanged e)
    {
        if (ammoText == null)
        {
            return;
        }

        ammoText.text = e.IsReloading ? "Перезарядка..." : $"{e.Current}/{e.Max}";
    }
}
