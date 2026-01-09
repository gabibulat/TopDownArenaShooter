using System.Collections;
using UnityEngine;
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
	
	private static readonly int Move = Animator.StringToHash("Move");
	private static readonly int Attack = Animator.StringToHash("Attack");
	private static readonly int Die = Animator.StringToHash("Die");

	//DEBUG
	[SerializeField] private EnemyData enemyData;

	public void Initialize(EnemyData data)
	{
		_agent.speed = data.chaseSpeed;
		_chaseSpeed = data.chaseSpeed;
		_agent.stoppingDistance = data.attackRange;
		_attackRange = data.attackRange;
		_detectionRange = data.detectionRange;
		_damage = data.damage;
		_attackCooldown = data.attackCooldown;
		_health.SetMaxHealth(enemyData.maxHealth);
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

		//DEBUG
		Initialize(enemyData);
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
			_animator.SetTrigger(Attack);
		}
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
		//Addressables.ReleaseInstance(gameObject);
		//DEBUG
		Destroy(gameObject);
	}

	private void OnDisable()
	{
		_health.Died -= OnDied;
	}

	// DEBUG 
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _detectionRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _attackRange);
	}
}