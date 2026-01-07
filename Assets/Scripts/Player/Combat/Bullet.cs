using UnityEngine;

public sealed class Bullet : MonoBehaviour
{
	[SerializeField] private LayerMask hitMask = ~0;

	private Vector3 _velocity;
	private int _damage;
	private float _dieAt;

	public void Launch(Vector3 position, Vector3 direction, float speed, int damage, float lifetime)
	{
		transform.position = position;

		direction.y = 0f;
		if (direction.sqrMagnitude <= 0.0001f)
		{
			direction = Vector3.forward;
		}

		direction.Normalize();

		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

		_velocity = direction * speed;
		_damage = damage;
		_dieAt = Time.time + lifetime;

		gameObject.SetActive(true);
	}

	private void Update()
	{
		var step = _velocity * Time.deltaTime;

		if (Physics.Raycast(transform.position, _velocity.normalized, out var hit, step.magnitude, hitMask, QueryTriggerInteraction.Ignore))
		{
			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				damageable.TakeDamage(_damage);
			}

			BulletPool.Instance.Release(this);
			return;
		}

		transform.position += step;

		if (Time.time >= _dieAt)
		{
			BulletPool.Instance.Release(this);
		}
	}
}