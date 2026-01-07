using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;
	[SerializeField] private WeaponInventory inventory;
	[SerializeField] private Transform muzzlePoint;

	[SerializeField] private bool autoReloadWhenEmpty = true;

	private bool _triggerHeld;
	private bool _isReloading;
	private float _nextFireTime;
	private float _reloadEndTime;

	private WeaponData _weapon;
	
	private void OnEnable()  => inventory.EquippedChanged += OnEquippedChanged;
	
	private void OnEquippedChanged(WeaponData weaponData)
	{
		_weapon = weaponData;
		_isReloading = false;
		_nextFireTime = 0f;
	}

	public void SetTriggerHeld(bool held) => _triggerHeld = held;

	private void Update()
	{
		if (!_triggerHeld && !_isReloading) return;

		if (_isReloading)
		{
			if (Time.time < _reloadEndTime) return;
			inventory.TryApplyReloadToCurrent();
			_isReloading = false;
		}

		if (_triggerHeld)
		{
			TryFire();
		}
	}

	private void TryFire()
	{
		if (Time.time < _nextFireTime) return;
		
		if (!inventory.TryConsumeFromCurrent(1))
		{
			TryReload();
			return;
		}
		
		_nextFireTime = Time.time + (1f / _weapon.fireRate);
		FireBullet();
		
		if (autoReloadWhenEmpty && !inventory.TryConsumeFromCurrent(0))
		{
			TryReload();
		}
	}

	private void TryReload()
	{
		if (_isReloading) return;
		if (!inventory.CanReloadCurrent()) return;

		_isReloading = true;
		_reloadEndTime = Time.time + _weapon.reloadTime;
	}

	private void FireBullet()
	{
		var dir = playerController.AimDirection.sqrMagnitude > 0.0001f
			? playerController.AimDirection
			: transform.forward;

		var bullet = BulletPool.Instance.Get();
		bullet.Launch(muzzlePoint.position, dir, _weapon.bulletSpeed, _weapon.damage, _weapon.bulletLifeTime);
	}
	
	private void OnDisable() => inventory.EquippedChanged -= OnEquippedChanged;
}