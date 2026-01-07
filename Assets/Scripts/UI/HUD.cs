using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
	[SerializeField] private TMP_Text _healthText;
	[SerializeField] private TMP_Text _ammoText;
	[SerializeField] private TMP_Text _weaponText;

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
		_weaponText.text = $"{weaponData.weaponId}";
	}

	private void AmmoUpdate(int ammoInMag, int ammoReserve)
	{
		_ammoText.text = $"{ammoInMag}/{ammoReserve}";
	}

	private void HealthUpdate(int current, int max)
	{
		_healthText.text = $"{current}/{max}";
	}
}