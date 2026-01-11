using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class GameController : MonoBehaviour
{
	[SerializeField] private Transform player;

	private readonly List<EntryPoint> _entryPoints = new();
	private Coroutine _runRoutine;

	private int _alive;
	private Queue<EnemyData> _queue;
	private const float DelayLevels = 2f;
	private LevelLoader _loader;

	private float _healthMul = 1f, _damageMul = 1f, _speedMul = 1f;
	public Action<int> NewLevel;
	public Action Won;

	private void Awake()
	{
		_entryPoints.AddRange(FindObjectsByType<EntryPoint>(FindObjectsSortMode.None));
		_entryPoints.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
	}

	private void OnEnable()
	{
		_loader = LevelLoader.Instance;
		if (_loader) _loader.LevelReady += OnLevelReady;
	}

	private void OnLevelReady(LevelData level)
	{
		if (_runRoutine != null) StopCoroutine(_runRoutine);
		_runRoutine = StartCoroutine(RunLevel(level));
	}

	private IEnumerator RunLevel(LevelData level)
	{
		if (_entryPoints.Count == 0) yield break;
		NewLevel?.Invoke(level.levelNumber);

		_alive = 0;
		BuildQueue(level);
		EvaluateDifficulty(level);

		var epIndex = 0;

		while (_queue.Count > 0)
		{
			while (level.maxAlive > 0 && _alive >= level.maxAlive) yield return null;

			for (int i = 0; i < level.spawnStep && _queue.Count > 0; i++)
			{
				while (level.maxAlive > 0 && _alive >= level.maxAlive) yield return null;
				var enemy = _queue.Dequeue();
				var ep = _entryPoints[epIndex].transform;
				epIndex = (epIndex + 1) % _entryPoints.Count;
				Spawn(enemy, ep);
			}

			yield return new WaitForSeconds(level.spawnRate);
		}

		while (_alive > 0) yield return null;
		yield return new WaitForSeconds(DelayLevels);

		if (!LevelLoader.Instance.LoadNextLevel()) Won?.Invoke();
	}

	private void BuildQueue(LevelData level)
	{
		_queue = new Queue<EnemyData>(256);

		foreach (var e in level.enemies)
		{
			if (e == null || !e.enemy) continue;
			for (int i = 0; i < e.count; i++)
			{
				_queue.Enqueue(e.enemy);
			}
		}
	}

	private void EvaluateDifficulty(LevelData level)
	{
		_healthMul = _damageMul = _speedMul = 1f;
		var t01 = Mathf.Clamp01((level.levelNumber - 1f) / Mathf.Max(1, level.difficultyMaxLevel - 1));
		_healthMul = level.difficultyProfile.Health(t01);
		_damageMul = level.difficultyProfile.Damage(t01);
		_speedMul = level.difficultyProfile.Speed(t01);
	}

	private void Spawn(EnemyData data, Transform point)
	{
		data.prefab.InstantiateAsync(point.position, point.rotation).Completed += h => OnSpawned(h, data);
	}

	private void OnSpawned(AsyncOperationHandle<GameObject> handle, EnemyData data)
	{
		if (handle.Status != AsyncOperationStatus.Succeeded) return;

		_alive++;

		var enemy = handle.Result.GetComponent<EnemyBehaviour>();
		enemy.InitializeFromSpawner(
			data,
			player,
			_healthMul,
			_damageMul,
			_speedMul,
			onReleasedAfterDeath: () => _alive = Mathf.Max(0, _alive - 1)
		);
	}

	private void OnDisable()
	{
		if (_loader) _loader.LevelReady -= OnLevelReady;
		_loader = null;
	}
}