using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text currencyText;

    private const string LanguageKey = "selected_language";

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyText;
            UpdateCurrencyText(CurrencyManager.Instance.CurrentCurrency);
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyText;
        }
    }

    private void Start()
    {
        if (CurrencyManager.Instance != null)
            UpdateCurrencyText(CurrencyManager.Instance.CurrentCurrency);
    }

    private void UpdateCurrencyText(int amount)
    {
        if (currencyText == null)
            return;

        if (IsRussian())
            currencyText.text = "" + amount;
        else
            currencyText.text = "" + amount;
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}