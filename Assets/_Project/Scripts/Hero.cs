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
    [SerializeField] private float _friction;

    [Header("Attack Boss Settings")]
    [SerializeField] private Transform _attackBossDestination;
    [SerializeField] private GameObject _swordGameObject;
    [SerializeField] private float _attackInterval;
    [SerializeField] private float _maxIdleTime;

    [Header("Basic Attack Settings")]
    [SerializeField] private float _dodgeDelay;
    [SerializeField] private Transform _basicAttackAnticipateDestination;

    [Header("Super Attack Settings")]
    [SerializeField] private Transform _superAttackAnticipateDestination;

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
    private IEnumerator _hitBossCoroutine = null;
    private float _startIdleTime;

    private void Awake()
    {
        _swordGameObject.SetActive(false);
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

    private void Update()
    {
        if (_rigidbody.velocity != Vector2.zero)
        {
            ResetIdleTimer();
            return;
        }

        if (IsOverMaxIdleTime())
        {
            ResetIdleTimer();
            TryHitBoss();
        }
    }

    private void ResetIdleTimer()
    {
        _startIdleTime = Time.time;
    }

    private bool IsOverMaxIdleTime()
    {
        float elapsedTime = Time.time - _startIdleTime;
        return elapsedTime > _maxIdleTime;
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
        else if (_moveInput == 0 && IsOnGround())
        {
            float movingDirection = Mathf.Sign(_rigidbody.velocity.x);
            float frictionAmount = Mathf.Min(_friction, Mathf.Abs(_rigidbody.velocity.x));
            float friction = -1 * movingDirection * frictionAmount;
            _rigidbody.AddForce(friction * Vector2.right, ForceMode2D.Impulse);
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
        Vector3 anticipateDestination = _basicAttackAnticipateDestination.position;
        AnticipateAttack(anticipateDestination);
    }

    public void AnticipateSuperAttack()
    {
        Vector3 anticipateDestination = _superAttackAnticipateDestination.position;
        AnticipateAttack(anticipateDestination);
    }

    public void DodgeBasicAttack()
    {
        if (IsDead()) return;

        DOVirtual.DelayedCall(_dodgeDelay, () => Dodge());
    }

    public void DodgeSuperAttack()
    {
        if (IsDead()) return;

        if (CanDodgeSuperAttack())
            Dodge();
        else
            DOVirtual.DelayedCall(_dodgeDelay, () => Dodge());
    }

    public bool CanDodgeSuperAttack()
    {
        Vector3 anticipateDestination = _superAttackAnticipateDestination.position;

        return (IsOnGround() && (IsWithinRadiusOfDestination(anticipateDestination)/* || IsOnRightOfDestination(anticipateDestination)*/));
    }

    // Triggered by Unity Event
    public void OnHitByProjectile()
    {
        StopHitBoss();
        CancelMovement();
    }

    private void Dodge()
    {
        if (IsOnGround())
        {
            StopHitBoss();
            ResetIdleTimer();

            TryJump();
        }
    }

    private void AnticipateAttack(Vector3 anticipateDestination)
    {
        if (IsDead()) return;

        ResetIdleTimer();
        StopHitBoss();

        MoveToDestination(anticipateDestination);
    }
    private void CancelMovement()
    {
        _moveInput = 0;
    }

    private void TryHitBoss()
    {
        StopHitBoss();

        _hitBossCoroutine = TryHitBossCoroutine();
        StartCoroutine(_hitBossCoroutine);
    }

    private IEnumerator TryHitBossCoroutine()
    {
        yield return MoveToDestinationCoroutine(_attackBossDestination.position);

        WaitForSeconds waitForSeconds = new WaitForSeconds(_attackInterval);
        while (true)
        {
            ResetIdleTimer();
            SwingSword();
            yield return waitForSeconds;
        }
    }

    private void SwingSword()
    {
        const float SWING_DISPLAY_TIME = 0.3f;

        _swordGameObject.SetActive(true);
        DOVirtual.DelayedCall(SWING_DISPLAY_TIME, () => _swordGameObject.SetActive(false));
    }

    private void StopHitBoss()
    {
        if (_hitBossCoroutine != null)
        {
            StopCoroutine(_hitBossCoroutine);
        }
    }

    private void MoveToDestination(Vector3 destination)
    {
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);

        _movementCoroutine = MoveToDestinationCoroutine(destination);
        StartCoroutine(_movementCoroutine);
    }

    private IEnumerator MoveToDestinationCoroutine(Vector3 destination)
    {
        while (!IsWithinRadiusOfDestination(destination))
        {
            if (IsOnLeftOfDestination(destination))
                MoveRight();

            if (IsOnRightOfDestination(destination))
                MoveLeft();

            yield return null;
        }

        CancelMovement();
    }

    private void OnDeath()
    {
        Debug.Log("Hero Died!");
        _rigidbody.freezeRotation = false;
    }

    private void TryJump()
    {
        _jumpInput = true;
    }

    private void MoveLeft()
    {
        _moveInput = -1;
    }

    private void MoveRight()
    {
        _moveInput = 1;
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
        bool withinXDistance = Math.Abs(destination.x - transform.position.x) <= _destinationRadius;
        bool withinYDistance = Math.Abs(destination.y - transform.position.y) <= _destinationRadius;

        return withinXDistance && withinYDistance;
    }

    private bool IsDead()
    {
        return _health.IsDead();
    }

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