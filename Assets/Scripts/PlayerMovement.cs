using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private CharacterController _controller;

    private Vector2 _moveInput;
    private bool _hasMoveInput;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        _hasMoveInput = _moveInput.sqrMagnitude > 0.0001f;
    }

    private void Update()
    {
        if (!_hasMoveInput) return;
        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        _controller.Move(move * moveSpeed * Time.deltaTime);
    }
}