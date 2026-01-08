using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField] private Image _healthBar;
	[SerializeField] private Image _weaponIcon;
	[SerializeField] private TMP_Text _ammoText;

	[SerializeField] private WeaponInventory _weaponInventory;
	[SerializeField] private Health _playerHealth;

	public static HUD Instance { get; private set; }

	private void Awake()
	{
		if (Instance && !Equals(Instance, this))
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		_playerHealth.Changed += HealthUpdate;
		_weaponInventory.AmmoChanged += AmmoUpdate;
		_weaponInventory.EquippedChanged += WeaponUpdate;
	}

	private void WeaponUpdate(WeaponData weaponData)
	{
		_weaponIcon.sprite = weaponData.icon;
	}

	private void AmmoUpdate(int ammoInMag, int ammoReserve)
	{
		_ammoText.text = $"{ammoInMag}/{ammoReserve}";
	}

	private void HealthUpdate(int current, int max)
	{
		_healthBar.fillAmount = Mathf.Clamp01((float)current / max);
	}
}