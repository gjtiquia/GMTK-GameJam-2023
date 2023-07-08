using UnityEngine;

public class KnockbackAction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _force;
    [SerializeField] private float _torque;
    [SerializeField, Range(0, 1)] private float _upwardTendency = 0;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;

    private bool _isKnockbackApplied;

    private void Awake()
    {
        _isKnockbackApplied = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isKnockbackApplied) return;

        if (other.TryGetComponent(out Knockback knockback))
        {
            _isKnockbackApplied = true;

            Vector2 direction = _rigidbody.velocity.normalized;
            direction += Vector2.up * _upwardTendency;

            knockback.ApplyKnockback(_force * direction, _torque);
        }
    }
}