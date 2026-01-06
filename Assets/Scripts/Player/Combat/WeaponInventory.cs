using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
	[SerializeField] private WeaponData startingWeapon;
	[SerializeField] private int startingReserveAmmo;
	private readonly Dictionary<WeaponData, WeaponState> _states = new();
	private readonly List<WeaponData> _owned = new();

	private WeaponData _currentWeapon;
	private WeaponState _currentState;

	public event Action<WeaponData, WeaponState> EquippedChanged;

	private void Start()
	{
		AddWeapon(startingWeapon, startingReserveAmmo, autoEquip: true);
	}

	private bool HasWeapon(WeaponData weapon) => weapon != null && _owned.Contains(weapon);

	public void AddWeapon(WeaponData weapon, int reserveAmmoGain = 0, bool autoEquip = false)
	{
		if (weapon == null) return;

		if (!HasWeapon(weapon))
		{
			_owned.Add(weapon);
		}

		if (!_states.TryGetValue(weapon, out var state))
		{
			state = new WeaponState(weapon.magazineSize, 0);
			_states.Add(weapon, state);
		}

		if (reserveAmmoGain > 0)
		{
			state.ammoReserve = Mathf.Clamp(state.ammoReserve + reserveAmmoGain, 0, weapon.maxReserveAmmo);
		}

		if (autoEquip)
		{
			Equip(weapon);
		}
	}

	private void Equip(WeaponData weapon)
	{
		if (weapon == null) return;

		if (!HasWeapon(weapon))
		{
			AddWeapon(weapon, 0, autoEquip: false);
		}

		_currentWeapon = weapon;
		_currentState = _states[weapon];
		EquippedChanged?.Invoke(_currentWeapon, _currentState);
	}

	public void AddAmmo(WeaponData weapon, int amount)
	{
		if (weapon == null || amount <= 0) return;

		if (!_states.TryGetValue(weapon, out var state))
		{
			if (!HasWeapon(weapon)) return;
			AddWeapon(weapon, 0, autoEquip: false);
			state = _states[weapon];
		}

		state.ammoReserve = Mathf.Clamp(state.ammoReserve + amount, 0, weapon.maxReserveAmmo);

		if (weapon == _currentWeapon)
		{
			EquippedChanged?.Invoke(_currentWeapon, _currentState);
		}
	}
}