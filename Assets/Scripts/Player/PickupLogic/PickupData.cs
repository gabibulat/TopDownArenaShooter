using UnityEngine;


public enum PickupType
{
	Heal,
	Ammo
}

[CreateAssetMenu(menuName = "Game/Pickup Data")]
public sealed class PickupData : ScriptableObject
{
	public PickupType type;
	[Min(1)] public int amount = 1;
	public WeaponData weaponType;
}