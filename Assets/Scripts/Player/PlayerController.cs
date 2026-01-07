using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;

	[SerializeField] private Camera mainCamera;
	[SerializeField] private float rotationSpeed = 20f;

	private CharacterController _controller;
	private Vector2 _moveInput;
	private Vector2 _aimInput;
	private bool _hasMoveInput;

	public Vector3 AimDirection { get; private set; } = Vector3.forward;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
	}

	public void SetMoveInput(Vector2 move)
	{
		_moveInput = move;
		_hasMoveInput = _moveInput.sqrMagnitude > 0.0001f;
	}

	public void SetLookInput(Vector2 look)
	{
		_aimInput = look;
	}

	private void Update()
	{
		Aim();
		if (_hasMoveInput)
		{
			Move();
		}
	}

	private void Move()
	{
		var move = new Vector3(_moveInput.x, 0f, _moveInput.y);
		if (move.sqrMagnitude > 1f)
		{
			move.Normalize();
		}

		_controller.Move(move * (moveSpeed * Time.deltaTime));
	}

	private void Aim()
	{
		if (!mainCamera) return;

		var ray = mainCamera.ScreenPointToRay(_aimInput);

		var plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
		if (!plane.Raycast(ray, out var enter))
			return;

		var aimPoint = ray.GetPoint(enter);
		var dir = aimPoint - transform.position;
		dir.y = 0f;

		if (dir.sqrMagnitude < 0.0001f)
			return;
		AimDirection = dir.normalized;

		var desired = Quaternion.LookRotation(AimDirection, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, desired, rotationSpeed * Time.deltaTime);
	}
}