using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
	public AssetReferenceGameObject prefab;
	[Range(0f, 1f)] public float dropChance = 0.5f;
	public int maxHealth;
	public float detectionRange;
	public float attackRange;
	public int damage;
	public float attackCooldown;
	public float chaseSpeed;
}