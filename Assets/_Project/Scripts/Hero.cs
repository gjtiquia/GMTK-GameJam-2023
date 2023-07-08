using UnityEngine;

public class Hero : MonoBehaviour
{
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
    }
}