using UnityEngine;

[CreateAssetMenu(menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
	public string weaponId;

	[Min(1)] public int damage;
	[Min(0.1f)] public float fireRate;
	[Min(0.1f)] public float bulletSpeed;
	[Min(0.1f)] public float bulletLifeTime;

	[Min(1)] public int magazineSize;
	[Min(0f)] public float reloadTime;
	[Min(0)] public int maxReserveAmmo;
}