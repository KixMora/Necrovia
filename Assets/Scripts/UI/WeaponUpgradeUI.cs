using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponInventory weaponInventory;

    [Header("Texts")]
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text damageLevelText;
    [SerializeField] private TMP_Text damageValueText;
    [SerializeField] private TMP_Text upgradeCostText;

    [Header("Button")]
    [SerializeField] private Button upgradeDamageButton;

    private WeaponData CurrentWeapon => weaponInventory != null ? weaponInventory.CurrentWeapon : null;

    private void Awake()
    {
        if (weaponInventory == null)
            weaponInventory = FindFirstObjectByType<PlayerWeaponInventory>();
    }

    private void OnEnable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged += HandleWeaponChanged;

        if (WeaponUpgradeManager.Instance != null)
            WeaponUpgradeManager.Instance.OnWeaponUpgraded += HandleWeaponUpgraded;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.AddListener(UpgradeDamage);

        RefreshUI();
    }

    private void OnDisable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged -= HandleWeaponChanged;

        if (WeaponUpgradeManager.Instance != null)
            WeaponUpgradeManager.Instance.OnWeaponUpgraded -= HandleWeaponUpgraded;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.RemoveListener(UpgradeDamage);
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        RefreshUI();
    }

    private void HandleWeaponUpgraded(WeaponData weapon)
    {
        RefreshUI();
    }

    private void UpgradeDamage()
    {
        if (WeaponUpgradeManager.Instance == null)
            return;

        WeaponUpgradeManager.Instance.TryUpgradeDamage(CurrentWeapon);
        RefreshUI();
    }

    public void RefreshUI()
    {
        WeaponData weapon = CurrentWeapon;

        if (weapon == null || WeaponUpgradeManager.Instance == null)
        {
            SetEmpty();
            return;
        }

        int level = WeaponUpgradeManager.Instance.GetDamageLevel(weapon);
        int currentDamage = WeaponUpgradeManager.Instance.GetCurrentDamage(weapon);
        int nextDamage = WeaponUpgradeManager.Instance.GetNextDamage(weapon);
        int cost = WeaponUpgradeManager.Instance.GetDamageUpgradeCost(weapon);
        bool canUpgrade = WeaponUpgradeManager.Instance.CanUpgradeDamage(weapon);

        if (weaponNameText != null)
            weaponNameText.text = weapon.weaponName;

        if (damageLevelText != null)
            damageLevelText.text = "Урон LVL: " + level + " / " + weapon.maxDamageLevel;

        if (damageValueText != null)
        {
            if (canUpgrade)
                damageValueText.text = "Урон: " + currentDamage + " → " + nextDamage;
            else
                damageValueText.text = "Урон: " + currentDamage + " MAX";
        }

        if (upgradeCostText != null)
        {
            if (canUpgrade)
                upgradeCostText.text = "Цена: " + cost;
            else
                upgradeCostText.text = "Максимум";
        }

        if (upgradeDamageButton != null)
            upgradeDamageButton.interactable = canUpgrade;
    }

    private void SetEmpty()
    {
        if (weaponNameText != null)
            weaponNameText.text = "";

        if (damageLevelText != null)
            damageLevelText.text = "";

        if (damageValueText != null)
            damageValueText.text = "";

        if (upgradeCostText != null)
            upgradeCostText.text = "";

        if (upgradeDamageButton != null)
            upgradeDamageButton.interactable = false;
    }
}