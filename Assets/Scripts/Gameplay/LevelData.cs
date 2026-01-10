using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
	public int levelNumber;
	public AssetReference levelScene;
	[Min(0.01f)] public float spawnRate;
	[Min(1)] public int spawnStep;
	[Min(0)] public int maxAlive;
	public EnemyEntry[] enemies;
	public DifficultyProfile difficultyProfile;
	[Min(1)] public int difficultyMaxLevel;
}

[Serializable]
public class EnemyEntry
{
	public EnemyData enemy;
	[Min(1)] public int count;
}