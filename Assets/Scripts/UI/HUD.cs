using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField] private Image _weaponIcon;
	[SerializeField] private TMP_Text _ammoText;
	[SerializeField] private WeaponInventory _weaponInventory;

	public static HUD Instance { get; private set; }

	private void Awake()
	{
		if (Instance && !Equals(Instance, this))
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		
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
}