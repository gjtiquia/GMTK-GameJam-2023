using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _fireVelocity;
    [SerializeField] private float _superAttackPowerupDuration;
    [SerializeField] private float _superAttackDuration;

    [Header("Debug")]
    [SerializeField] private bool _disableAttack;

    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private GameObject _bossDiedPopup;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Transform _superFirePosition;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _superAttackInstance;

    private Sequence _superSequence;

    private void Awake()
    {
        _bossDiedPopup.SetActive(false);
    }

    private void Start()
    {
        _health.OnDeathCallback += () => _bossDiedPopup.SetActive(true);
    }

    public void Fire()
    {
        if (_disableAttack)
            return;

        GameObject projectileInstance = Instantiate(_projectilePrefab, _firePosition.position, Quaternion.identity);
        if (projectileInstance.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = _fireVelocity;
        }
        else Debug.LogError("Projectile does not have a rigidbody!");

        // TODO : Refactor with ObjectPool
        // DOVirtual.DelayedCall(5, () => Destroy(projectileInstance));
    }

    public void Super()
    {
        if (_disableAttack)
            return;

        _superAttackInstance.SetActive(false);

        if (_superSequence != null && _superSequence.IsPlaying())
            _superSequence.Kill();

        _superSequence = DOTween.Sequence();
        _superSequence
            .AppendInterval(_superAttackPowerupDuration) // TODO : Animation from single line to huge lazer beam
            .AppendCallback(() => _superAttackInstance.SetActive(true))
            .AppendInterval(_superAttackDuration)
            .AppendCallback(() => _superAttackInstance.SetActive(false));
    }
}
