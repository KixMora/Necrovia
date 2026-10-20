using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponIconUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponInventory weaponInventory;
    [SerializeField] private Image weaponIconImage;

    [Header("Optional")]
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private Sprite fallbackIcon;

    private void Awake()
    {
        if (weaponInventory == null)
            weaponInventory = FindFirstObjectByType<PlayerWeaponInventory>();
    }

    private void OnEnable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged += HandleWeaponChanged;
    }

    private void OnDisable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged -= HandleWeaponChanged;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (weaponInventory != null)
            HandleWeaponChanged(weaponInventory.CurrentWeapon);
        else
            HandleWeaponChanged(null);
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        UpdateIcon(weapon);
        UpdateName(weapon);
    }

    private void UpdateIcon(WeaponData weapon)
    {
        if (weaponIconImage == null)
            return;

        if (weapon != null && weapon.icon != null)
        {
            weaponIconImage.enabled = true;
            weaponIconImage.sprite = weapon.icon;
            weaponIconImage.color = Color.white;
            return;
        }

        if (fallbackIcon != null)
        {
            weaponIconImage.enabled = true;
            weaponIconImage.sprite = fallbackIcon;
            weaponIconImage.color = Color.white;
        }
        else
        {
            weaponIconImage.enabled = false;
        }
    }

    private void UpdateName(WeaponData weapon)
    {
        if (weaponNameText == null)
            return;

        if (weapon != null)
            weaponNameText.text = weapon.weaponName;
        else
            weaponNameText.text = "";
    }
}