using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LoadPlayerPrefs : MonoBehaviour
{
    private const string PlayerPrefsKey = "language";

    private void Awake()
    {
        StartCoroutine(ApplySavedLanguage());
    }

    private IEnumerator ApplySavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;
        if (!PlayerPrefs.HasKey(PlayerPrefsKey)) yield break;
        var code = PlayerPrefs.GetString(PlayerPrefsKey);
        var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (locale) LocalizationSettings.SelectedLocale = locale;
    }
}