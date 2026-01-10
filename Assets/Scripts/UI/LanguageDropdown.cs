using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;

public class LanguageDropdown : MonoBehaviour
{
	[SerializeField] private TMP_Dropdown dropdown;
	private const string TableCollection = "UI_Text";
	private const string KeyEnglish = "lang_english";
	private const string KeyCroatian = "lang_croatian";
	private const string LocaleEnglish = "en";
	private const string LocaleCroatian = "hr";
	private const string PlayerPrefsKey = "language";

	private Locale _en;
	private Locale _hr;
	private void Awake()
	{
		StartCoroutine(ApplySavedLocale());
	}

	private IEnumerator ApplySavedLocale()
	{
		yield return LocalizationSettings.InitializationOperation;

		if (PlayerPrefs.HasKey(PlayerPrefsKey))
		{
			var code = PlayerPrefs.GetString(PlayerPrefsKey);
			var locale = LocalizationSettings.AvailableLocales.GetLocale(code);
			if (locale) LocalizationSettings.SelectedLocale = locale;
		}
	}
	
	private IEnumerator Start()
	{
		yield return LocalizationSettings.InitializationOperation;

		_en = LocalizationSettings.AvailableLocales.GetLocale(LocaleEnglish);
		_hr = LocalizationSettings.AvailableLocales.GetLocale(LocaleCroatian);
		
		BuildOptions();
		SyncDropdownToCurrentLocale();

		dropdown.onValueChanged.AddListener(OnDropdownChanged);
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void BuildOptions()
	{
		dropdown.ClearOptions();

		var options = new List<string>
		{
			LocalizationSettings.StringDatabase.GetLocalizedString(TableCollection, KeyEnglish),
			LocalizationSettings.StringDatabase.GetLocalizedString(TableCollection, KeyCroatian)
		};

		dropdown.AddOptions(options);
	}

	private void OnDropdownChanged(int index)
	{
		var target = (index == 1) ? _hr : _en;
		LocalizationSettings.SelectedLocale = target;
		PlayerPrefs.SetString(PlayerPrefsKey, target.Identifier.Code);
	}

	private void OnLocaleChanged(Locale _)
	{
		BuildOptions();
		SyncDropdownToCurrentLocale();
	}

	private void SyncDropdownToCurrentLocale()
	{
		var isHr = LocalizationSettings.SelectedLocale != null &&
		           LocalizationSettings.SelectedLocale.Identifier.Code == _hr.Identifier.Code;

		dropdown.SetValueWithoutNotify(isHr ? 1 : 0);
		dropdown.RefreshShownValue();
	}

	private void OnDestroy()
	{
		dropdown?.onValueChanged.RemoveListener(OnDropdownChanged);
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}
}