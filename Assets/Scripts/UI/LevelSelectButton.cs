using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public sealed class LevelSelectButton : MonoBehaviour
{
	[SerializeField] private Button button;
	[SerializeField] private TMP_Text labelTMP;
	[SerializeField] private LocalizedString levelLabel;
	private int _levelIndex;

	private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

	public void Init(int levelIndex)
	{
		_levelIndex = levelIndex;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(OnClicked);
		_ = RefreshLabelAsync();
	}

	private void OnClicked()
	{
		LevelLoader.Instance.LoadLevelByIndex(_levelIndex);
	}

	private async void OnLocaleChanged(Locale _)
	{
		await RefreshLabelAsync();
	}

	private async Task RefreshLabelAsync()
	{
		levelLabel.Arguments = new object[] { _levelIndex + 1 };
		var op = levelLabel.GetLocalizedStringAsync();
		await op.Task;
		labelTMP.text = op.Result;
	}
	
	private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
}