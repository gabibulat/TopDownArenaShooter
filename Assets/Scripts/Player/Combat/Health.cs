using System;
using UnityEngine;

public interface IDamageable
{
	void TakeDamage(int amount);
}

public sealed class Health : MonoBehaviour, IDamageable
{
	[SerializeField] private int max = 100;
	private int _current;
	public event Action<int, int> Changed;
	public event Action Died;

	private bool IsDead => _current <= 0;

	private void Start()
	{
		_current = max;
		Changed?.Invoke(_current, max);
	}

	public void TakeDamage(int amount)
	{
		if (IsDead) return;

		_current = Mathf.Max(0, _current - amount);
		Changed?.Invoke(_current, max);

		if (_current == 0)
		{
			Died?.Invoke();
		}
	}

	public bool Heal(int amount)
	{
		if (IsDead) return false;
		if (_current == max) return false;
		_current = Mathf.Min(max, _current + amount);
		Changed?.Invoke(_current, max);
		return true;
	}
}