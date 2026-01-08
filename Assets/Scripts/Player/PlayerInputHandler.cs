using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	private PlayerController _playerController;
	private PlayerCombat _playerCombat;

	private void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		_playerCombat = GetComponent<PlayerCombat>();
	}

	private void OnMove(InputValue value)
	{
		_playerController?.SetMoveInput(value.Get<Vector2>());
	}

	private void OnLook(InputValue value)
	{
		_playerController?.SetLookInput(value.Get<Vector2>());
	}

	private void OnFire(InputValue value)
	{
		_playerCombat.SetTriggerHeld(value.isPressed);
	}
}