using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;


public class WeaponUpgradeRowUI : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private WeaponUpgradeType upgradeType;

    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button upgradeButton;
    [Header("Cost Colors")]
    [SerializeField] private Color enoughMoneyColor = Color.white;
    [SerializeField] private Color notEnoughMoneyColor = Color.red;
    [SerializeField] private Color maxColor = Color.gray;

    [Header("Upgrade Feedback")]
    [SerializeField] private CanvasGroup rowCanvasGroup;
    [SerializeField] private RectTransform valueTextRect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip upgradeSuccessClip;
    [SerializeField] private float flashDuration = 0.18f;
    [SerializeField] private float scalePunch = 1.18f;

    private Coroutine feedbackRoutine;
    private const string LanguageKey = "selected_language";

    private WeaponData currentWeapon;

    private void OnEnable()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(Upgrade);

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.RemoveListener(Upgrade);

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        Refresh();
    }

    public void Setup(WeaponData weapon)
    {
        currentWeapon = weapon;
        Refresh();
    }

    public void Refresh()
    {
        if (currentWeapon == null || WeaponUpgradeManager.Instance == null)
        {
            SetEmpty();
            return;
        }

        switch (upgradeType)
        {
            case WeaponUpgradeType.Damage:
                RefreshDamage();
                break;

            case WeaponUpgradeType.Magazine:
                RefreshMagazine();
                break;

            case WeaponUpgradeType.FireRate:
                RefreshFireRate();
                break;

            case WeaponUpgradeType.Reload:
                RefreshReload();
                break;
        }
    }

    private void RefreshDamage()
    {
        var manager = WeaponUpgradeManager.Instance;

        int level = manager.GetDamageLevel(currentWeapon);
        int current = manager.GetCurrentDamage(currentWeapon);
        int next = manager.GetNextDamage(currentWeapon);
        int cost = manager.GetDamageUpgradeCost(currentWeapon);
        bool canUpgrade = manager.CanUpgradeDamage(currentWeapon);

        SetTexts(
            GetTitle(),
            level,
            currentWeapon.maxDamageLevel,
            current.ToString(),
            next.ToString(),
            cost,
            canUpgrade
        );
    }

    private void RefreshMagazine()
    {
        var manager = WeaponUpgradeManager.Instance;

        int level = manager.GetMagazineLevel(currentWeapon);
        int current = manager.GetCurrentMagazineSize(currentWeapon);
        int next = manager.GetNextMagazineSize(currentWeapon);
        int cost = manager.GetMagazineUpgradeCost(currentWeapon);
        bool canUpgrade = manager.CanUpgradeMagazine(currentWeapon);

        SetTexts(
            GetTitle(),
            level,
            currentWeapon.maxMagazineLevel,
            current.ToString(),
            next.ToString(),
            cost,
            canUpgrade
        );
    }

    private void RefreshFireRate()
    {
        var manager = WeaponUpgradeManager.Instance;

        int level = manager.GetFireRateLevel(currentWeapon);
        float current = manager.GetCurrentFireRate(currentWeapon);
        float next = manager.GetNextFireRate(currentWeapon);
        int cost = manager.GetFireRateUpgradeCost(currentWeapon);
        bool canUpgrade = manager.CanUpgradeFireRate(currentWeapon);

        SetTexts(
            GetTitle(),
            level,
            currentWeapon.maxFireRateLevel,
            current.ToString("0.0"),
            next.ToString("0.0"),
            cost,
            canUpgrade
        );
    }
    private void Awake()
    {
        if (rowCanvasGroup == null)
            rowCanvasGroup = GetComponent<CanvasGroup>();

        if (rowCanvasGroup == null)
            rowCanvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (valueTextRect == null && valueText != null)
            valueTextRect = valueText.GetComponent<RectTransform>();

        if (audioSource == null)
            audioSource = GetComponentInParent<AudioSource>();
    }
    private void RefreshReload()
    {
        var manager = WeaponUpgradeManager.Instance;

        int level = manager.GetReloadLevel(currentWeapon);
        float current = manager.GetCurrentReloadTime(currentWeapon);
        float next = manager.GetNextReloadTime(currentWeapon);
        int cost = manager.GetReloadUpgradeCost(currentWeapon);
        bool canUpgrade = manager.CanUpgradeReload(currentWeapon);

        SetTexts(
            GetTitle(),
            level,
            currentWeapon.maxReloadLevel,
            current.ToString("0.0") + "s",
            next.ToString("0.0") + "s",
            cost,
            canUpgrade
        );
    }

    private void SetTexts(string title, int level, int maxLevel, string currentValue, string nextValue, int cost, bool canUpgrade)
    {
        bool hasMoney = CurrencyManager.Instance != null && CurrencyManager.Instance.CurrentCurrency >= cost;

        if (titleText != null)
            titleText.text = title;

        if (levelText != null)
            levelText.text = "LVL " + level + " / " + maxLevel;

        if (valueText != null)
        {
            if (canUpgrade)
                valueText.text = currentValue + " → " + nextValue;
            else
                valueText.text = currentValue + " " + GetMaxText();
        }

        if (costText != null)
        {
            if (canUpgrade)
            {
                costText.text = cost.ToString();

                if (hasMoney)
                    costText.color = enoughMoneyColor;
                else
                    costText.color = notEnoughMoneyColor;
            }
            else
            {
                costText.text = GetMaxText();
                costText.color = maxColor;
            }
        }

        if (upgradeButton != null)
            upgradeButton.interactable = canUpgrade && hasMoney;
    }

    private void Upgrade()
    {
        if (currentWeapon == null || WeaponUpgradeManager.Instance == null)
            return;

        bool upgraded = false;

        switch (upgradeType)
        {
            case WeaponUpgradeType.Damage:
                upgraded = WeaponUpgradeManager.Instance.TryUpgradeDamage(currentWeapon);
                break;

            case WeaponUpgradeType.Magazine:
                upgraded = WeaponUpgradeManager.Instance.TryUpgradeMagazine(currentWeapon);
                break;

            case WeaponUpgradeType.FireRate:
                upgraded = WeaponUpgradeManager.Instance.TryUpgradeFireRate(currentWeapon);
                break;

            case WeaponUpgradeType.Reload:
                upgraded = WeaponUpgradeManager.Instance.TryUpgradeReload(currentWeapon);
                break;
        }

        Refresh();

        if (upgraded)
            PlayUpgradeFeedback();
    }

    private string GetTitle()
    {
        bool ru = IsRussian();

        switch (upgradeType)
        {
            case WeaponUpgradeType.Damage:
                return ru ? "УРОН" : "DAMAGE";

            case WeaponUpgradeType.Magazine:
                return ru ? "МАГАЗИН" : "MAGAZINE";

            case WeaponUpgradeType.FireRate:
                return ru ? "СКОРОСТЬ" : "FIRE RATE";

            case WeaponUpgradeType.Reload:
                return ru ? "ПЕРЕЗАРЯДКА" : "RELOAD";

            default:
                return ru ? "УЛУЧШЕНИЕ" : "UPGRADE";
        }
    }

    private string GetMaxText()
    {
        return IsRussian() ? "МАКС" : "MAX";
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }

    private void SetEmpty()
    {
        if (titleText != null)
            titleText.text = "";

        if (levelText != null)
            levelText.text = "";

        if (valueText != null)
            valueText.text = "";

        if (costText != null)
            costText.text = "";

        if (upgradeButton != null)
            upgradeButton.interactable = false;
    }
    private void PlayUpgradeFeedback()
    {
        if (upgradeSuccessClip != null && audioSource != null)
            audioSource.PlayOneShot(upgradeSuccessClip);

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(UpgradeFeedbackRoutine());
    }

    private IEnumerator UpgradeFeedbackRoutine()
    {
        if (rowCanvasGroup != null)
            rowCanvasGroup.alpha = 1f;

        Vector3 originalScale = Vector3.one;

        if (valueTextRect != null)
            originalScale = valueTextRect.localScale;

        float timer = 0f;
        float halfDuration = flashDuration * 0.5f;

        while (timer < halfDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / halfDuration;

            if (rowCanvasGroup != null)
                rowCanvasGroup.alpha = Mathf.Lerp(1f, 0.55f, t);

            if (valueTextRect != null)
                valueTextRect.localScale = Vector3.Lerp(originalScale, originalScale * scalePunch, t);

            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / halfDuration;

            if (rowCanvasGroup != null)
                rowCanvasGroup.alpha = Mathf.Lerp(0.55f, 1f, t);

            if (valueTextRect != null)
                valueTextRect.localScale = Vector3.Lerp(originalScale * scalePunch, originalScale, t);

            yield return null;
        }

        if (rowCanvasGroup != null)
            rowCanvasGroup.alpha = 1f;

        if (valueTextRect != null)
            valueTextRect.localScale = originalScale;

        feedbackRoutine = null;
    }
}