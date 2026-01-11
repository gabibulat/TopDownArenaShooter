using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerPickupInteraction : MonoBehaviour
{
	private Health _health;
	private WeaponInventory _weapons;

	private void Awake()
	{
		_health = GetComponent<Health>();
		_weapons = GetComponent<WeaponInventory>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<Pickup>(out var pickup)) return;
		if (!pickup.Data) return;

		if (Apply(pickup.Data))
		{
			Addressables.ReleaseInstance(other.gameObject);
		}
	}

	private bool Apply(PickupData data)
	{
		switch (data.type)
		{
			case PickupType.Heal:
				return _health.Heal(data.amount);

			case PickupType.Ammo:
				if (data.weaponType)
				{
					return _weapons.AddAmmo(data.weaponType, data.amount);
				}

				break;
		}

		return false;
	}
}