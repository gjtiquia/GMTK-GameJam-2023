using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _fireVelocity;

    [Header("References")]
    [SerializeField] private Transform _firePosition;
    [SerializeField] private GameObject _projectilePrefab;

    public void Fire()
    {
        GameObject projectileInstance = Instantiate(_projectilePrefab, _firePosition.position, Quaternion.identity);
        if (projectileInstance.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = _fireVelocity;
        }
        else Debug.LogError("Projectile does not have a rigidbody!");

        // TODO : Refactor with ObjectPool
        // DOVirtual.DelayedCall(5, () => Destroy(projectileInstance));
    }
}
