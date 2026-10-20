using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private TMP_Text ammoText;

    private void Update()
    {
        if (playerShooting == null || ammoText == null)
            return;

        if (playerShooting.CurrentWeapon == null)
        {
            ammoText.text = "";
            return;
        }

        if (playerShooting.IsReloading)
        {
            ammoText.text = "Reloading...";
            return;
        }

        ammoText.text = playerShooting.CurrentAmmo + " / " + playerShooting.MagazineSize;
    }
}