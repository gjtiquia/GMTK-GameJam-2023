using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;

    private void OnValidate()
    {
        Utilities.IsPropertyNull(_projectilePrefab);
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
        GameObject projectileInstance = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        if (projectileInstance.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = new Vector2(5, 0);
        }
        else Debug.LogError("Projectile does not have a rigidbody!");
    }
}
