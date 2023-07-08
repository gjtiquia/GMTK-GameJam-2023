using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _destinationRadius;

    [Header("Basic Attack Settings")]
    [SerializeField] private Transform _basicAttackAnticipateDestination;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Health _health;

    private int _moveInput = 0; // 1 => Right, -1 => Left
    private IEnumerator _movementCoroutine = null;

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
    }

    public void AnticipateBasicAttack()
    {
        if (IsDead()) return;

        Vector3 destination = _basicAttackAnticipateDestination.position;

        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);

        if (IsOnLeftOfDestination(destination))
        {
            // Set as moving towards destination
            // save the coroutine to cancel
            _movementCoroutine = MoveRightUntilReachDestination(destination);
            StartCoroutine(_movementCoroutine);
            return;
        }

        if (IsOnRightOfDestination(destination))
        {
            // Set as moving towards destination
            // save the coroutine to cancel

            _movementCoroutine = MoveLeftUntilReachDestination(destination);
            StartCoroutine(_movementCoroutine);
            return;
        }

        // if is on the right of the destination
        // move left until reach destination
    }

    public void DodgeBasicAttack()
    {
        // TODO : Jump
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
        while (!IsWithinRadiusOfDestination(destination))
        {
            _moveInput = 1;
            yield return null;
        }

        _moveInput = 0;
    }

    private IEnumerator MoveLeftUntilReachDestination(Vector3 destination)
    {
        while (!IsWithinRadiusOfDestination(destination))
        {
            _moveInput = -1;
            yield return null;
        }

        _moveInput = 0;
    }

    private bool IsDead() => _health.IsDead();
    private bool IsMoving()
    {
        return _moveInput != 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _destinationRadius);
    }
}