using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hero : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _fallGravityMultiplier;

    [Header("Pathfinding Settings")]
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _destinationRadius;

    [Header("Basic Attack Settings")]
    [SerializeField] private Transform _basicAttackAnticipateDestination;

    [Header("On Ground Hitbox Settings")]
    [SerializeField] private Vector2 _boxExtents;
    [SerializeField] private Vector2 _boxOffset;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Health _health;

    private float _gravityScale;
    private int _moveInput = 0; // 1 => Right, -1 => Left
    private bool _jumpInput = false;
    private IEnumerator _movementCoroutine = null;

    private void Awake()
    {
        _gravityScale = _rigidbody.gravityScale;
    }

    private void Start()
    {
        _health.OnDeathCallback += OnDeath;
    }

    private void OnDestroy()
    {
        _health.OnDeathCallback -= OnDeath;
    }

    private void FixedUpdate()
    {
        if (IsDead()) return;

        // Reference: Improve Your Platformer with Forces | Examples in Unity - Dawnosaur
        // https://www.youtube.com/watch?v=KbtcEVCM7bw&ab_channel=Dawnosaur

        if (_moveInput != 0)
        {
            float targetVelocity = _moveInput * _topSpeed;
            float velocityDiff = targetVelocity - _rigidbody.velocity.x;
            float acceleration = _acceleration;
            float movement = velocityDiff * acceleration;
            _rigidbody.AddForce(movement * Vector2.right);
        }

        if (_jumpInput && IsOnGround())
        {
            _rigidbody.AddForce(_jumpForce * Vector2.up, ForceMode2D.Impulse);
            _jumpInput = false;
        }

        if (_rigidbody.velocity.y < 0)
            _rigidbody.gravityScale = _fallGravityMultiplier * _gravityScale;
        else
            _rigidbody.gravityScale = _gravityScale;
    }

    public void AnticipateBasicAttack()
    {
        if (IsDead()) return;

        Vector3 destination = _basicAttackAnticipateDestination.position;

        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);

        if (IsOnLeftOfDestination(destination))
        {
            _movementCoroutine = MoveRightUntilReachDestination(destination);
            StartCoroutine(_movementCoroutine);
            return;
        }

        if (IsOnRightOfDestination(destination))
        {
            _movementCoroutine = MoveLeftUntilReachDestination(destination);
            StartCoroutine(_movementCoroutine);
            return;
        }
    }

    public void DodgeBasicAttack()
    {
        if (IsOnGround())
            TryJump();
    }

    public void MoveLeft()
    {
        _moveInput = -1;
    }

    public void MoveRight()
    {
        _moveInput = 1;
    }

    public void CancelMovement()
    {
        // Debug.Log("Cancel Movement");

        _moveInput = 0;
    }

    public void TryJump()
    {
        _jumpInput = true;
    }

    private void OnDeath()
    {
        Debug.Log("Hero Died!");
        _rigidbody.freezeRotation = false;
    }

    private bool IsOnLeftOfDestination(Vector3 destination)
    {
        return transform.position.x < destination.x;
    }

    private bool IsOnRightOfDestination(Vector3 destination)
    {
        return transform.position.x > destination.x;
    }

    private bool IsWithinRadiusOfDestination(Vector3 destination)
    {
        return Math.Abs(destination.x - transform.position.x) <= _destinationRadius;
    }

    private IEnumerator MoveRightUntilReachDestination(Vector3 destination)
    {
        MoveRight();

        while (!IsWithinRadiusOfDestination(destination))
            yield return null;

        CancelMovement();
    }

    private IEnumerator MoveLeftUntilReachDestination(Vector3 destination)
    {
        MoveLeft();

        while (!IsWithinRadiusOfDestination(destination))
            yield return null;

        CancelMovement();
    }

    private bool IsDead() => _health.IsDead();
    private bool IsMoving()
    {
        return _moveInput != 0;
    }
    private bool IsOnGround()
    {
        Collider2D collider = Physics2D.OverlapBox((Vector2)transform.position + _boxOffset, _boxExtents, 0);

        if (collider != null && collider.tag == "Ground")
            return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _destinationRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)_boxOffset, _boxExtents);
    }
}