using UnityEngine;

public class DamageAction : MonoBehaviour
{
    [SerializeField] private int _damageAmount;

    private bool _isDamageApplied;

    private void OnEnable()
    {
        _isDamageApplied = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDamageApplied) return;

        if (other.TryGetComponent(out Health health))
        {
            _isDamageApplied = true;
            health.DealDamage(_damageAmount);
        }
    }
}