using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public sealed class EnemyBehaviour : MonoBehaviour
{
	[SerializeField] private float destinationUpdateRate = 0.15f;
	[SerializeField] private float faceTargetSpeed = 12f;
	[SerializeField] private float destroyDelay = 2f;

	private Transform _target;
	private NavMeshAgent _agent;
	private Health _health;
	private Collider _collider;
	private Animator _animator;

	private float _nextDestinationTime;
	private float _nextAttackTime;
	private bool _dead;

	private float _detectionRange;
	private int _damage;
	private float _attackCooldown;
	private float _attackRange;
	private float _chaseSpeed;

	private Action _onReleasedAfterDeath;

	private static readonly int Move = Animator.StringToHash("Move");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int Die = Animator.StringToHash("Die");
	
	public void InitializeFromSpawner(EnemyData data, Transform target, float healthMul, float damageMul,
		float speedMul, System.Action onReleasedAfterDeath)
	{
		_target = target;
		_onReleasedAfterDeath = onReleasedAfterDeath;

		var scaledSpeed = data.chaseSpeed * speedMul;
		_chaseSpeed = scaledSpeed;
		_agent.speed = scaledSpeed;

		_agent.stoppingDistance = data.attackRange;
		_attackRange = data.attackRange;
		_detectionRange = data.detectionRange;
		_attackCooldown = data.attackCooldown;
		_damage = Mathf.RoundToInt(data.damage * damageMul);

		var hp = Mathf.RoundToInt(data.maxHealth * healthMul);
		_health.SetMaxHealth(hp);
	}

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_health = GetComponent<Health>();
		_collider = GetComponent<Collider>();
		_animator = GetComponent<Animator>();

		var player = GameObject.FindGameObjectWithTag("Player");
		if (player) _target = player.transform;
		_agent.autoBraking = true;
	}

	private void OnEnable()
	{
		_health.Died += OnDied;
	}

	private void Update()
	{
		if (_dead) return;
		if (!_target) return;
		if (!_agent.enabled || !_agent.isOnNavMesh) return;

		var dist = Vector3.Distance(transform.position, _target.position);

		if (dist > _detectionRange)
		{
			StopAgent();
			return;
		}

		if (dist <= _attackRange)
		{
			StopAgent();
			FaceTarget();
			TryAttack();
			return;
		}

		ResumeAgent();
		UpdateDestination();

		var isMoving = _agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance &&
		               _agent.velocity.sqrMagnitude > 0.01f;
		_animator.SetBool(Move, isMoving);
	}

	private void UpdateDestination()
	{
		if (Time.time < _nextDestinationTime) return;
		_nextDestinationTime = Time.time + destinationUpdateRate;

		_agent.SetDestination(_target.position);
	}

	private void TryAttack()
	{
		if (Time.time < _nextAttackTime) return;
		_nextAttackTime = Time.time + _attackCooldown;
		if (_target.TryGetComponent<IDamageable>(out var damageable))
		{
			damageable.TakeDamage(_damage);
		}

		_animator.SetTrigger(Attack);
	}

	private void FaceTarget()
	{
		var dir = _target.position - transform.position;
		dir.y = 0f;
		if (dir.sqrMagnitude < 0.0001f) return;

		var targetRot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, faceTargetSpeed * Time.deltaTime);
	}

	private void StopAgent()
	{
		if (!_agent.enabled) return;
		if (_agent.isStopped) return;

		_agent.isStopped = true;
		_agent.ResetPath();
	}

	private void ResumeAgent()
	{
		if (!_agent.enabled) return;
		if (!_agent.isStopped) return;

		_agent.isStopped = false;
		_agent.speed = _chaseSpeed;
	}

	private void OnDied()
	{
		if (_dead) return;
		_dead = true;

		if (_agent.enabled)
		{
			_agent.isStopped = true;
			_agent.ResetPath();
			_agent.enabled = false;
		}

		_collider.enabled = false;

		_animator.SetTrigger(Die);

		StartCoroutine(ReleaseAfterDelay());
	}

	private IEnumerator ReleaseAfterDelay()
	{
		yield return new WaitForSeconds(destroyDelay);
		_onReleasedAfterDeath?.Invoke();
		_onReleasedAfterDeath = null;
		Addressables.ReleaseInstance(gameObject);
	}

	private void OnDisable()
	{
		_health.Died -= OnDied;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _detectionRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _attackRange);
	}
}