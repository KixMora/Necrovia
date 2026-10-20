using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class WeaponUpgradePanelUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;
    [Header("Upgrade Rows")]
    [SerializeField] private WeaponUpgradeRowUI[] upgradeRows;
    [Header("Weapons")]
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();
    [SerializeField] private int selectedWeaponIndex = 0;

    [Header("Weapon Preview")]
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private Sprite fallbackWeaponIcon;

    [Header("Weapon Switch Buttons")]
    [SerializeField] private Button previousWeaponButton;
    [SerializeField] private Button nextWeaponButton;

    [Header("Damage Upgrade")]
    [SerializeField] private TMP_Text damageTitleText;
    [SerializeField] private TMP_Text damageLevelText;
    [SerializeField] private TMP_Text damageValueText;
    [SerializeField] private TMP_Text damageCostText;
    [SerializeField] private Button upgradeDamageButton;

    [Header("Currency")]
    [SerializeField] private TMP_Text currentScrapText;

    [Header("Optional Buttons")]
    [SerializeField] private Button closeButton;

    private const string LanguageKey = "selected_language";

    private List<WeaponData> GetOwnedWeapons()
    {
        List<WeaponData> ownedWeapons = new List<WeaponData>();

        foreach (WeaponData weapon in weapons)
        {
            if (weapon == null)
                continue;

            if (WeaponShopManager.Instance == null)
            {
                ownedWeapons.Add(weapon);
                continue;
            }

            if (WeaponShopManager.Instance.IsWeaponOwned(weapon))
                ownedWeapons.Add(weapon);
        }

        return ownedWeapons;
    }
        private WeaponData CurrentWeapon
        {
            get
            {
                List<WeaponData> ownedWeapons = GetOwnedWeapons();

                if (ownedWeapons.Count == 0)
                    return null;

                selectedWeaponIndex = Mathf.Clamp(selectedWeaponIndex, 0, ownedWeapons.Count - 1);
                return ownedWeapons[selectedWeaponIndex];
            }
        }

    private void Awake()
    {
        if (root == null)
            root = gameObject;
    }

    private void OnEnable()
    {
        if (WeaponUpgradeManager.Instance != null)
            WeaponUpgradeManager.Instance.OnWeaponUpgraded += HandleWeaponUpgraded;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.AddListener(UpgradeDamage);

        if (previousWeaponButton != null)
            previousWeaponButton.onClick.AddListener(SelectPreviousWeapon);

        if (nextWeaponButton != null)
            nextWeaponButton.onClick.AddListener(SelectNextWeapon);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        RefreshUI();
    }

    private void OnDisable()
    {
        if (WeaponUpgradeManager.Instance != null)
            WeaponUpgradeManager.Instance.OnWeaponUpgraded -= HandleWeaponUpgraded;

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;

        if (upgradeDamageButton != null)
            upgradeDamageButton.onClick.RemoveListener(UpgradeDamage);

        if (previousWeaponButton != null)
            previousWeaponButton.onClick.RemoveListener(SelectPreviousWeapon);

        if (nextWeaponButton != null)
            nextWeaponButton.onClick.RemoveListener(SelectNextWeapon);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);
    }

    public void Show()
    {
        if (root != null)
            root.SetActive(true);

        RefreshUI();
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void SelectNextWeapon()
    {
        List<WeaponData> ownedWeapons = GetOwnedWeapons();

        if (ownedWeapons.Count == 0)
            return;

        selectedWeaponIndex++;

        if (selectedWeaponIndex >= ownedWeapons.Count)
            selectedWeaponIndex = 0;

        RefreshUI();
    }

    public void SelectPreviousWeapon()
    {
        List<WeaponData> ownedWeapons = GetOwnedWeapons();

        if (ownedWeapons.Count == 0)
            return;

        selectedWeaponIndex--;

        if (selectedWeaponIndex < 0)
            selectedWeaponIndex = ownedWeapons.Count - 1;

        RefreshUI();
    }

    private void HandleWeaponUpgraded(WeaponData weapon)
    {
        RefreshUI();
    }

    private void HandleCurrencyChanged(int amount)
    {
        RefreshCurrency();
        RefreshUpgradeButtonState();
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
        RefreshWeaponPreview();
        RefreshDamageUpgrade();
        RefreshCurrency();
        RefreshStaticTexts();
        RefreshUpgradeRows();
    }
    private void RefreshUpgradeRows()
    {
        WeaponData weapon = CurrentWeapon;

        if (upgradeRows == null)
            return;

        foreach (WeaponUpgradeRowUI row in upgradeRows)
        {
            if (row != null)
                row.Setup(weapon);
        }
    }
    private void RefreshWeaponPreview()
    {
        WeaponData weapon = CurrentWeapon;

        if (weaponNameText != null)
        {
            if (weapon != null)
                weaponNameText.text = weapon.weaponName;
            else
                weaponNameText.text = "";
        }

        if (weaponIconImage != null)
        {
            if (weapon != null && weapon.icon != null)
            {
                weaponIconImage.enabled = true;
                weaponIconImage.sprite = weapon.icon;
            }
            else if (fallbackWeaponIcon != null)
            {
                weaponIconImage.enabled = true;
                weaponIconImage.sprite = fallbackWeaponIcon;
            }
            else
            {
                weaponIconImage.enabled = false;
            }
        }

        bool hasMultipleWeapons = GetOwnedWeapons().Count > 1;

        if (previousWeaponButton != null)
            previousWeaponButton.interactable = hasMultipleWeapons;

        if (nextWeaponButton != null)
            nextWeaponButton.interactable = hasMultipleWeapons;
    }

    private void RefreshDamageUpgrade()
    {
        WeaponData weapon = CurrentWeapon;

        if (weapon == null || WeaponUpgradeManager.Instance == null)
        {
            SetDamageEmpty();
            return;
        }

        int level = WeaponUpgradeManager.Instance.GetDamageLevel(weapon);
        int currentDamage = WeaponUpgradeManager.Instance.GetCurrentDamage(weapon);
        int nextDamage = WeaponUpgradeManager.Instance.GetNextDamage(weapon);
        int cost = WeaponUpgradeManager.Instance.GetDamageUpgradeCost(weapon);
        bool canUpgrade = WeaponUpgradeManager.Instance.CanUpgradeDamage(weapon);

        if (damageLevelText != null)
            damageLevelText.text = "LVL " + level + " / " + weapon.maxDamageLevel;

        if (damageValueText != null)
        {
            if (canUpgrade)
                damageValueText.text = currentDamage + " → " + nextDamage;
            else
                damageValueText.text = currentDamage + " MAX";
        }

        if (damageCostText != null)
        {
            if (canUpgrade)
                damageCostText.text = cost.ToString();
            else
                damageCostText.text = IsRussian() ? "МАКС" : "MAX";
        }

        RefreshUpgradeButtonState();
    }

    private void RefreshUpgradeButtonState()
    {
        if (upgradeDamageButton == null)
            return;

        WeaponData weapon = CurrentWeapon;

        if (weapon == null || WeaponUpgradeManager.Instance == null || CurrencyManager.Instance == null)
        {
            upgradeDamageButton.interactable = false;
            return;
        }

        bool canUpgrade = WeaponUpgradeManager.Instance.CanUpgradeDamage(weapon);
        int cost = WeaponUpgradeManager.Instance.GetDamageUpgradeCost(weapon);
        bool hasMoney = CurrencyManager.Instance.CurrentCurrency >= cost;

        upgradeDamageButton.interactable = canUpgrade && hasMoney;
    }

    private void RefreshCurrency()
    {
        if (currentScrapText == null)
            return;

        if (CurrencyManager.Instance != null)
            currentScrapText.text = CurrencyManager.Instance.CurrentCurrency.ToString();
        else
            currentScrapText.text = "0";
    }

    private void RefreshStaticTexts()
    {
        if (damageTitleText != null)
            damageTitleText.text = IsRussian() ? "УРОН" : "DAMAGE";
    }

    private void SetDamageEmpty()
    {
        if (weaponNameText != null)
            weaponNameText.text = "";

        if (damageLevelText != null)
            damageLevelText.text = "";

        if (damageValueText != null)
            damageValueText.text = "";

        if (damageCostText != null)
            damageCostText.text = "";

        if (upgradeDamageButton != null)
            upgradeDamageButton.interactable = false;
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}