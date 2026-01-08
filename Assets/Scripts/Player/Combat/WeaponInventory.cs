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

	public event Action<WeaponData> EquippedChanged;
	public event Action<int, int> AmmoChanged; 

	private void Start()
	{
		AddWeapon(startingWeapon, startingReserveAmmo, autoEquip: true);
	}

	private bool HasWeapon(WeaponData weapon) => weapon && _owned.Contains(weapon);

	private void AddWeapon(WeaponData weapon, int reserveAmmoGain = 0, bool autoEquip = false)
	{
		if (!weapon) return;

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
		if (!weapon) return;

		if (!HasWeapon(weapon))
		{
			AddWeapon(weapon, 0, autoEquip: false);
		}

		_currentWeapon = weapon;
		_currentState = _states[weapon];
		EquippedChanged?.Invoke(_currentWeapon);
		AmmoChanged?.Invoke(_currentState.ammoInMag, _currentState.ammoReserve); 
	}

	public bool AddAmmo(WeaponData weapon, int amount)
	{
		if (amount <= 0) return false;

		if (!_states.TryGetValue(weapon, out var state))
		{
			if (!HasWeapon(weapon)) return false;
			AddWeapon(weapon, 0, autoEquip: false);
			state = _states[weapon];
		}

		if (state.ammoReserve == weapon.maxReserveAmmo) return false;
		state.ammoReserve = Mathf.Clamp(state.ammoReserve + amount, 0, weapon.maxReserveAmmo);

		if (weapon == _currentWeapon)
		{
			AmmoChanged?.Invoke(_currentState.ammoInMag, _currentState.ammoReserve); 
		}
		return true;
	}
	
	public bool TryConsumeFromCurrent(int amount = 1)
	{
		if (amount <= 0) return true;

		if (_currentState.ammoInMag < amount) return false;

		_currentState.ammoInMag -= amount;
		AmmoChanged?.Invoke(_currentState.ammoInMag, _currentState.ammoReserve); 
		return true;
	}

	public void TryApplyReloadToCurrent()
	{
		if (_currentState.ammoInMag >= _currentWeapon.magazineSize) return;
		if (_currentState.ammoReserve <= 0) return;

		var need = _currentWeapon.magazineSize - _currentState.ammoInMag;
		var take = Mathf.Min(need, _currentState.ammoReserve);

		_currentState.ammoInMag += take;
		_currentState.ammoReserve -= take;

		AmmoChanged?.Invoke(_currentState.ammoInMag, _currentState.ammoReserve); 
	}
	
	public bool CanReloadCurrent()
	{
		if (_currentState.ammoInMag >= _currentWeapon.magazineSize) return false;
		return _currentState.ammoReserve > 0;
	}
}