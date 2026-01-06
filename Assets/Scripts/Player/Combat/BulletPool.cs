using System.Collections.Generic;
using UnityEngine;

public sealed class BulletPool : MonoBehaviour
{
	public static BulletPool Instance { get; private set; }

	[SerializeField] private Bullet bulletPrefab;
	[SerializeField] private int size;

	private readonly Queue<Bullet> _pool = new();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		for (int i = 0; i < size; i++)
		{
			var bullet = Instantiate(bulletPrefab, transform);
			bullet.gameObject.SetActive(false);
			_pool.Enqueue(bullet);
		}
	}

	public Bullet Get()
	{
		if (_pool.Count == 0)
		{
			var b = Instantiate(bulletPrefab, transform);
			b.gameObject.SetActive(false);
			_pool.Enqueue(b);
		}

		var bullet = _pool.Dequeue();
		bullet.gameObject.SetActive(true);
		return bullet;
	}

	public void Release(Bullet bullet)
	{
		bullet.gameObject.SetActive(false);
		_pool.Enqueue(bullet);
	}
}