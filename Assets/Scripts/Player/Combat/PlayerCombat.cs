using UnityEngine;

public interface IDamageable
{
	void TakeDamage(int amount);
}

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
	private WeaponState _state;

	private void Awake()
	{
		inventory.EquippedChanged += OnEquippedChanged;
	}

	private void OnEquippedChanged(WeaponData weaponData, WeaponState weaponState)
	{
		_weapon = weaponData;
		_state = weaponState;
		_isReloading = false;
		_nextFireTime = 0f;
	}

	public void SetTriggerHeld(bool held) => _triggerHeld = held;

	private void TryReload()
	{
		if (_isReloading) return;
		if (_state.ammoInMag >= _weapon.magazineSize) return;
		if (_state.ammoReserve <= 0) return;

		_isReloading = true;
		_reloadEndTime = Time.time + _weapon.reloadTime;
	}

	private void Update()
	{
		if (!_triggerHeld && !_isReloading) return;

		if (_isReloading)
		{
			if (Time.time < _reloadEndTime)
				return;

			int need = _weapon.magazineSize - _state.ammoInMag;
			int take = Mathf.Min(need, _state.ammoReserve);

			_state.ammoInMag += take;
			_state.ammoReserve -= take;

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

		if (_state.ammoInMag <= 0)
		{
			if (autoReloadWhenEmpty) TryReload();
			return;
		}

		_nextFireTime = Time.time + (1f / _weapon.fireRate);
		_state.ammoInMag--;

		FireBullet();

		if (autoReloadWhenEmpty && _state.ammoInMag <= 0)
			TryReload();
	}

	private void FireBullet()
	{
		Vector3 dir = playerController.AimDirection.sqrMagnitude > 0.0001f
			? playerController.AimDirection
			: transform.forward;

		Bullet bullet = BulletPool.Instance.Get();
		bullet.Launch(muzzlePoint.position, dir, _weapon.bulletSpeed, _weapon.damage, _weapon.bulletLifeTime);
	}
}