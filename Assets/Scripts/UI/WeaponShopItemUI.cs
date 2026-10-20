using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class WeaponShopItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponStatsText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image costIconImage;
    [Header("Optional")]
    [SerializeField] private Sprite fallbackWeaponIcon;

    [Header("Colors")]
    [SerializeField] private Color enoughMoneyColor = new Color(0.86f, 0.86f, 0.82f, 1f);
    [SerializeField] private Color notEnoughMoneyColor = new Color(0.50f, 0.22f, 0.20f, 1f);
    [SerializeField] private Color ownedColor = new Color(0.55f, 0.80f, 0.55f, 1f);

    private const string LanguageKey = "selected_language";

    private WeaponData weapon;
    private WeaponShopPanelUI ownerPanel;

    private void OnEnable()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(Buy);
    }

    private void OnDisable()
    {
        if (buyButton != null)
            buyButton.onClick.RemoveListener(Buy);
    }

    public void Setup(WeaponData weaponData, WeaponShopPanelUI panel)
    {
        weapon = weaponData;
        ownerPanel = panel;

        Refresh();
    }

    public void Refresh()
    {
        if (weapon == null)
        {
            SetEmpty();
            return;
        }

        bool owned = WeaponShopManager.Instance != null && WeaponShopManager.Instance.IsWeaponOwned(weapon);
        bool canBuy = WeaponShopManager.Instance != null && WeaponShopManager.Instance.CanBuyWeapon(weapon);
        bool hasMoney = CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCurrency >= weapon.buyCost;

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

        if (weaponNameText != null)
            weaponNameText.text = weapon.weaponName;

        if (weaponStatsText != null)
        {
            weaponStatsText.text =
                GetDamageLabel() + ": " + weapon.damage +
                "   " + GetMagazineLabel() + ": " + weapon.magazineSize +
                "\n" +
                GetFireRateLabel() + ": " + weapon.fireRate.ToString("0.0") +
                "   " + GetReloadLabel() + ": " + weapon.reloadTime.ToString("0.0") + "s";
        }

        if (costText != null)
        {
            if (owned)
            {
                costText.text = "";
            }
            else
            {
                costText.text = weapon.buyCost.ToString();
                costText.color = hasMoney ? enoughMoneyColor : notEnoughMoneyColor;
            }
        }

        if (statusText != null)
        {
            if (owned)
            {
                statusText.text = IsRussian() ? "КУПЛЕНО" : "OWNED";
                statusText.color = ownedColor;
            }
            else if (!hasMoney)
            {
                statusText.text = IsRussian() ? "НЕ ХВАТАЕТ" : "NOT ENOUGH";
                statusText.color = notEnoughMoneyColor;
            }
            else
            {
                statusText.text = "";
            }
        }

        if (costIconImage != null)
            costIconImage.gameObject.SetActive(!owned);

        if (buyButton != null)
        {
            // Если оружие куплено — полностью скрываем кнопку
            buyButton.gameObject.SetActive(!owned);

            if (!owned)
            {
                buyButton.interactable = canBuy;

                TMP_Text buttonText = buyButton.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                    buttonText.text = IsRussian() ? "КУПИТЬ" : "BUY";
            }
        }
    }

    private void Buy()
    {
        if (weapon == null || WeaponShopManager.Instance == null)
            return;

        bool success = WeaponShopManager.Instance.TryBuyWeapon(weapon);

        if (success)
        {
            Refresh();

            if (ownerPanel != null)
                ownerPanel.RefreshUI();
        }
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
            buyButton.interactable = false;
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }

    private string GetDamageLabel()
    {
        return IsRussian() ? "Урон" : "DMG";
    }

    private string GetMagazineLabel()
    {
        return IsRussian() ? "Маг" : "MAG";
    }

    private string GetFireRateLabel()
    {
        return IsRussian() ? "Скор" : "RATE";
    }

    private string GetReloadLabel()
    {
        return IsRussian() ? "Перез" : "RLD";
    }
}