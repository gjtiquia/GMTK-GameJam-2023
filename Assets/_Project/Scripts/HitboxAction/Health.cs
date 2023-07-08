using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action<float> OnNormalizedCurrentHPChangedCallback;
    public Action OnDeathCallback;

    [SerializeField] private int _maxHP = 100;

    private int _currentHP
    {
        get => m_currentHP;
        set
        {
            m_previousHP = m_currentHP;
            m_currentHP = Mathf.Clamp(value, 0, _maxHP);

            if (m_currentHP != m_previousHP)
                OnCurrentHPChange();
        }
    }
    private int m_previousHP;
    private int m_currentHP;

    public void DealDamage(int amount)
    {
        _currentHP -= amount;
    }

    public float GetNormalizedHP()
    {
        return (float)_currentHP / (float)_maxHP;
    }

    private void Awake()
    {
        _currentHP = _maxHP;
    }

    private void OnCurrentHPChange()
    {
        Debug.Log($"HP: {_currentHP}/{_maxHP}");
        OnNormalizedCurrentHPChangedCallback?.Invoke(GetNormalizedHP());

        if (_currentHP <= 0)
            OnDeathCallback?.Invoke();
    }
}