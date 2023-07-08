using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _fireVelocity;

    [Header("References")]
    [SerializeField] private Transform _firePosition;
    [SerializeField] private GameObject _projectilePrefab;

    private void OnValidate()
    {
        Utilities.IsPropertyNull(_projectilePrefab);
        Utilities.IsPropertyNull(_firePosition);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        GameObject projectileInstance = Instantiate(_projectilePrefab, _firePosition.position, Quaternion.identity);
        if (projectileInstance.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = _fireVelocity;
        }
        else Debug.LogError("Projectile does not have a rigidbody!");
    }
}
