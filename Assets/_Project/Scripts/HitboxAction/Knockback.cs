using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    public void ApplyKnockback(Vector2 force, float torque)
    {
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
        _rigidbody.AddTorque(torque, ForceMode2D.Impulse);
    }
}