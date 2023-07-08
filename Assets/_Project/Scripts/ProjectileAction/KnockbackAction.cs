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

    private void OnEnable()
    {
        _isKnockbackApplied = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isKnockbackApplied) return;

        if (other.TryGetComponent(out Knockback knockback))
        {
            _isKnockbackApplied = true;
            knockback.ApplyKnockback(_force * GetKnockbackDirection(), _torque);
        }
    }

    private Vector2 GetKnockbackDirection()
    {
        Vector2 direction = Vector2.right;

        if (_rigidbody != null)
            direction = _rigidbody.velocity.normalized;

        direction += Vector2.up * _upwardTendency;

        return direction;
    }
}