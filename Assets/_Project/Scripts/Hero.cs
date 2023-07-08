using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _acceleration;


    [Header("Basic Attack Settings")]
    [SerializeField] private Transform _basicAttackAnticipateDestination;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Health _health;

    private void Start()
    {
        _health.OnDeathCallback += OnDeath;
    }

    private void OnDestroy()
    {
        _health.OnDeathCallback -= OnDeath;
    }

    public void AnticipateBasicAttack()
    {
        // if dead, return

        // if is already moving towards a destination, return

        Vector3 destination = _basicAttackAnticipateDestination.position;

        // if is within radius of the destination, return

        if (IsOnLeftOfDestination(destination))
        {
            // Set as moving towards destination
            // save the coroutine to cancel

            StartCoroutine(MoveRightUntilReachDestination(destination));
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

    private IEnumerator MoveRightUntilReachDestination(Vector3 destination)
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (IsOnLeftOfDestination(destination))
        {
            yield return waitForFixedUpdate;

            // Reference: Improve Your Platformer with Forces | Examples in Unity - Dawnosaur
            // https://www.youtube.com/watch?v=KbtcEVCM7bw&ab_channel=Dawnosaur

            float targetVelocity = 1 * _topSpeed;
            float velocityDiff = targetVelocity - _rigidbody.velocity.x;
            float acceleration = _acceleration;
            float movement = velocityDiff * acceleration;
            _rigidbody.AddForce(movement * Vector2.right);
        }

        // set as finished moving towards destination
    }
}