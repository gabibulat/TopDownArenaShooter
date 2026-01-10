using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField] private Image weaponIcon;
	[SerializeField] private TMP_Text ammoText;
	[SerializeField] private TMP_Text levelText;
	[SerializeField] private LocalizedString levelLabel;
	[SerializeField] private WeaponInventory weaponInventory;
	[SerializeField] private GameController gameController;
	private int _lastLevelNumber;
	private static HUD _instance;

	private void OnEnable()
	{
		weaponInventory.AmmoChanged += AmmoUpdate;
		weaponInventory.EquippedChanged += WeaponUpdate;
		gameController.NewLevel += LevelUpdate;
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void Awake()
	{
		if (_instance && !Equals(_instance, this))
		{
			Destroy(gameObject);
			return;
		}

		_instance = this;
	}

	private async void OnLocaleChanged(Locale _)
	{
		await UpdateLevelTextAsync(_lastLevelNumber);
	}

	private void LevelUpdate(int levelNumber)
	{
		_lastLevelNumber = levelNumber;
		_ = UpdateLevelTextAsync(levelNumber);
	}

	private async Task UpdateLevelTextAsync(int levelNumber)
	{
		levelLabel.Arguments = new object[] { levelNumber };
		var op = levelLabel.GetLocalizedStringAsync();
		await op.Task;
		levelText.text = op.Result;
	}

	private void WeaponUpdate(WeaponData weaponData)
	{
		weaponIcon.sprite = weaponData.icon;
	}

	private void AmmoUpdate(int ammoInMag, int ammoReserve)
	{
		ammoText.text = $"{ammoInMag}/{ammoReserve}";
	}

	private void OnDisable()
	{
		weaponInventory.AmmoChanged -= AmmoUpdate;
		weaponInventory.EquippedChanged -= WeaponUpdate;
		gameController.NewLevel -= LevelUpdate;
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}
}