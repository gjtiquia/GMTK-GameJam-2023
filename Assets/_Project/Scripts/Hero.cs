using UnityEngine;

public class Hero : MonoBehaviour
{
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

    private void OnDeath()
    {
        Debug.Log("Hero Died!");
        _rigidbody.freezeRotation = false;
    }
}