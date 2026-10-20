using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    private const string LanguageKey = "selected_language";

    public void SetRussian()
    {
        StartCoroutine(SetLanguage("ru"));
    }

    public void SetEnglish()
    {
        StartCoroutine(SetLanguage("en"));
    }

    private IEnumerator SetLanguage(string langCode)
    {
        // ждём инициализацию (на всякий случай)
        yield return LocalizationSettings.InitializationOperation;

        Locale targetLocale = null;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == langCode)
            {
                targetLocale = locale;
                break;
            }
        }

        if (targetLocale == null)
        {
            Debug.LogWarning("Locale not found: " + langCode);
            yield break;
        }

        // применяем язык
        LocalizationSettings.SelectedLocale = targetLocale;

        // сохраняем
        PlayerPrefs.SetString(LanguageKey, langCode);
        PlayerPrefs.Save();
    }

    // 🔥 опционально — получить текущий язык
    public string GetCurrentLanguage()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code;

        return "ru";
    }
}