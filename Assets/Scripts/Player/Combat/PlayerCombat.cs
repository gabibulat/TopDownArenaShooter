using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
	[SerializeField] private Transform muzzlePoint;
	[SerializeField] private AudioSource gunAudioSource;
	
	private Animator _animator;
	private PlayerController _playerController;
	private WeaponInventory _inventory;
	private WeaponData _weapon;
	private LineRenderer _lineRenderer;

	private bool _triggerHeld;
	private bool _isReloading;
	private float _nextFireTime;
	private float _reloadEndTime;

	private static readonly int Shoot = Animator.StringToHash("Shoot");

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_playerController = GetComponent<PlayerController>();
		_inventory = GetComponent<WeaponInventory>();
		_lineRenderer = GetComponent<LineRenderer>();
		_inventory.EquippedChanged += OnEquippedChanged;
		_lineRenderer.positionCount = 2;

		_inventory.EquippedChanged += OnEquippedChanged;
	}

	private void OnEquippedChanged(WeaponData weaponData)
	{
		_weapon = weaponData;
		_isReloading = false;
		_nextFireTime = 0f;
	}

	public void SetTriggerHeld(bool held) => _triggerHeld = held;

	private void Update()
	{
		AimLineUpdate();

		if (!_triggerHeld && !_isReloading) return;

		if (_isReloading)
		{
			if (Time.time < _reloadEndTime) return;
			_inventory.TryApplyReloadToCurrent();
			_isReloading = false;
		}

		if (_triggerHeld)
		{
			TryFire();
		}
	}

	private void AimLineUpdate()
	{
		var aim = _playerController.AimDirection;
		if (!(aim.sqrMagnitude > 0.0001f)) return;
		aim.y = 0f;
		var start = muzzlePoint.position;
		const int maxDistance = 20;
		var end = start + aim.normalized * maxDistance;
		if (Physics.Raycast(start, aim.normalized, out var hit, maxDistance)) end = hit.point;
		_lineRenderer.SetPosition(0, start);
		_lineRenderer.SetPosition(1, end);
	}

	private void TryFire()
	{
		if (Time.time < _nextFireTime) return;

		if (!_inventory.TryConsumeFromCurrent(1))
		{
			TryReload();
			return;
		}

		_nextFireTime = Time.time + (1f / _weapon.fireRate);
		FireBullet();

		if (!_inventory.TryConsumeFromCurrent(0))
		{
			TryReload();
		}
	}

	private void TryReload()
	{
		if (_isReloading) return;
		if (!_inventory.CanReloadCurrent()) return;

		_isReloading = true;
		_reloadEndTime = Time.time + _weapon.reloadTime;
	}

	private void FireBullet()
	{
		var dir = _playerController.AimDirection.sqrMagnitude > 0.0001f
			? _playerController.AimDirection
			: transform.forward;

		var bullet = BulletPool.Instance.Get();
		gunAudioSource.PlayOneShot(gunAudioSource.clip);
		bullet.Launch(muzzlePoint.position, dir, _weapon.bulletSpeed, _weapon.damage, _weapon.bulletLifeTime);
		_animator.SetTrigger(Shoot);
	}

	private void OnDisable() => _inventory.EquippedChanged -= OnEquippedChanged;
}