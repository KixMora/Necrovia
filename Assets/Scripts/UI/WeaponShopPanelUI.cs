using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class WeaponShopPanelUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Weapons")]
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();
    [SerializeField] private int selectedWeaponIndex = 0;

    [Header("Header")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button closeButton;

    [Header("Currency")]
    [SerializeField] private TMP_Text scrapText;

    [Header("Weapon Preview")]
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private Sprite fallbackWeaponIcon;

    [Header("Switch Buttons")]
    [SerializeField] private Button previousWeaponButton;
    [SerializeField] private Button nextWeaponButton;

    [Header("Stats")]
    [SerializeField] private TMP_Text weaponStatsText;

    [Header("Cost")]
    [SerializeField] private Image costIconImage;
    [SerializeField] private TMP_Text costText;

    [Header("Status")]
    [SerializeField] private TMP_Text statusText;

    [Header("Buy")]
    [SerializeField] private Button buyButton;

    [Header("Colors")]
    [SerializeField] private Color enoughMoneyColor = new Color(0.86f, 0.86f, 0.82f, 1f);
    [SerializeField] private Color notEnoughMoneyColor = new Color(0.50f, 0.22f, 0.20f, 1f);
    [SerializeField] private Color ownedColor = new Color(0.55f, 0.80f, 0.55f, 1f);
    [SerializeField] private Color normalStatusColor = new Color(0.86f, 0.86f, 0.82f, 1f);

    private const string LanguageKey = "selected_language";

    private WeaponData CurrentWeapon
    {
        get
        {
            if (weapons == null || weapons.Count == 0)
                return null;

            selectedWeaponIndex = Mathf.Clamp(selectedWeaponIndex, 0, weapons.Count - 1);
            return weapons[selectedWeaponIndex];
        }
    }

    private void Awake()
    {
        if (root == null)
            root = gameObject;
    }

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

        if (WeaponShopManager.Instance != null)
            WeaponShopManager.Instance.OnWeaponPurchased += HandleWeaponPurchased;

        if (previousWeaponButton != null)
            previousWeaponButton.onClick.AddListener(SelectPreviousWeapon);

        if (nextWeaponButton != null)
            nextWeaponButton.onClick.AddListener(SelectNextWeapon);

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyCurrentWeapon);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;

        if (WeaponShopManager.Instance != null)
            WeaponShopManager.Instance.OnWeaponPurchased -= HandleWeaponPurchased;

        if (previousWeaponButton != null)
            previousWeaponButton.onClick.RemoveListener(SelectPreviousWeapon);

        if (nextWeaponButton != null)
            nextWeaponButton.onClick.RemoveListener(SelectNextWeapon);

        if (buyButton != null)
            buyButton.onClick.RemoveListener(BuyCurrentWeapon);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
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
        if (weapons == null || weapons.Count == 0)
            return;

        selectedWeaponIndex++;

        if (selectedWeaponIndex >= weapons.Count)
            selectedWeaponIndex = 0;

        RefreshUI();
    }

    public void SelectPreviousWeapon()
    {
        if (weapons == null || weapons.Count == 0)
            return;

        selectedWeaponIndex--;

        if (selectedWeaponIndex < 0)
            selectedWeaponIndex = weapons.Count - 1;

        RefreshUI();
    }

    private void BuyCurrentWeapon()
    {
        WeaponData weapon = CurrentWeapon;

        if (weapon == null || WeaponShopManager.Instance == null)
            return;

        bool success = WeaponShopManager.Instance.TryBuyWeapon(weapon);

        if (success)
            RefreshUI();
    }

    public void RefreshUI()
    {
        // RefreshStaticTexts();
        RefreshCurrency();
        RefreshWeapon();
        RefreshSwitchButtons();
    }

    private void RefreshStaticTexts()
    {
        if (titleText != null)
            titleText.text = IsRussian() ? "МАГАЗИН" : "SHOP";
    }

    private void RefreshCurrency()
    {
        if (scrapText == null)
            return;

        if (CurrencyManager.Instance != null)
            scrapText.text = CurrencyManager.Instance.CurrentCurrency.ToString();
        else
            scrapText.text = "0";
    }

    private void RefreshWeapon()
    {
        WeaponData weapon = CurrentWeapon;

        if (weapon == null)
        {
            SetEmpty();
            return;
        }

        bool owned = WeaponShopManager.Instance != null && WeaponShopManager.Instance.IsWeaponOwned(weapon);
        bool hasMoney = CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCurrency >= weapon.buyCost;
        bool canBuy = WeaponShopManager.Instance != null && WeaponShopManager.Instance.CanBuyWeapon(weapon);

        RefreshWeaponPreview(weapon);
        RefreshStats(weapon);
        RefreshCost(weapon, owned, hasMoney);
        RefreshStatus(owned, hasMoney);
        RefreshBuyButton(owned, canBuy);
    }

    private void RefreshWeaponPreview(WeaponData weapon)
    {
        if (weaponNameText != null)
            weaponNameText.text = weapon.weaponName;

        if (weaponIconImage != null)
        {
            if (weapon.icon != null)
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
    }

    private void RefreshStats(WeaponData weapon)
    {
        if (weaponStatsText == null)
            return;

        string damageLabel = IsRussian() ? "Урон" : "Damage";
        string magazineLabel = IsRussian() ? "Магазин" : "Magazine";
        string fireRateLabel = IsRussian() ? "Скорость" : "Fire rate";
        string reloadLabel = IsRussian() ? "Перезарядка" : "Reload";

        weaponStatsText.text =
            damageLabel + ": " + weapon.damage +
            "     " +
            magazineLabel + ": " + weapon.magazineSize +
            "\n" +
            fireRateLabel + ": " + weapon.fireRate.ToString("0.0") +
            "     " +
            reloadLabel + ": " + weapon.reloadTime.ToString("0.0") + "s";
    }

    private void RefreshCost(WeaponData weapon, bool owned, bool hasMoney)
    {
        if (costIconImage != null)
            costIconImage.gameObject.SetActive(!owned);

        if (costText == null)
            return;

        if (owned)
        {
            costText.text = "";
            return;
        }

        costText.text = weapon.buyCost.ToString();
        costText.color = hasMoney ? enoughMoneyColor : notEnoughMoneyColor;
    }

    private void RefreshStatus(bool owned, bool hasMoney)
    {
        if (statusText == null)
            return;

        if (owned)
        {
            statusText.text = IsRussian() ? "КУПЛЕНО" : "OWNED";
            statusText.color = ownedColor;
        }
        else if (!hasMoney)
        {
            statusText.text = IsRussian() ? "НЕ ХВАТАЕТ ХЛАМА" : "NOT ENOUGH SCRAP";
            statusText.color = notEnoughMoneyColor;
        }
        else
        {
            statusText.text = IsRussian() ? "ДОСТУПНО ДЛЯ ПОКУПКИ" : "AVAILABLE";
            statusText.color = normalStatusColor;
        }
    }

    private void RefreshBuyButton(bool owned, bool canBuy)
    {
        if (buyButton == null)
            return;

        buyButton.gameObject.SetActive(!owned);

        if (!owned)
            buyButton.interactable = canBuy;
    }

    private void RefreshSwitchButtons()
    {
        bool hasMultipleWeapons = weapons != null && weapons.Count > 1;

        if (previousWeaponButton != null)
            previousWeaponButton.interactable = hasMultipleWeapons;

        if (nextWeaponButton != null)
            nextWeaponButton.interactable = hasMultipleWeapons;
    }

    private void SetEmpty()
    {
        if (weaponIconImage != null)
            weaponIconImage.enabled = false;

        if (weaponNameText != null)
            weaponNameText.text = "";

        if (weaponStatsText != null)
            weaponStatsText.text = "";

        if (costText != null)
            costText.text = "";

        if (statusText != null)
            statusText.text = "";

        if (buyButton != null)
            buyButton.gameObject.SetActive(false);
    }

    private void HandleCurrencyChanged(int amount)
    {
        RefreshCurrency();
        RefreshWeapon();
    }

    private void HandleWeaponPurchased(WeaponData weapon)
    {
        RefreshWeapon();
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        RefreshUI();
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}