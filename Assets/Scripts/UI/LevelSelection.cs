using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public sealed class LevelSelection : MonoBehaviour
{
	[SerializeField] private LevelCatalog levelCatalog;
	[SerializeField] private GameObject buttonPrefab;
	[SerializeField] private Transform contentRoot;

	private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

	private void Awake() => BuildButtons();

	private void OnLocaleChanged(Locale _) => BuildButtons();

	private void BuildButtons()
	{
		for (int i = contentRoot.childCount - 1; i >= 0; i--)
			Destroy(contentRoot.GetChild(i).gameObject);

		for (int i = 0; i < levelCatalog.levels.Length; i++)
		{
			var go = Instantiate(buttonPrefab, contentRoot);
			var ui = go.GetComponent<LevelSelectButton>();

			ui.Init(levelIndex: i);
		}
	}
	
	private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
}