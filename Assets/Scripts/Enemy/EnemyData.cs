using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
	public AssetReferenceGameObject prefab;
	public int maxHealth;
	public float detectionRange;
	public float attackRange;
	public int damage;
	public float attackCooldown;
	public float chaseSpeed;
}