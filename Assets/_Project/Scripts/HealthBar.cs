using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private Health _health;

    [Header("Internal References")]
    [SerializeField] private Image _healthUI;

    private void Start()
    {
        _healthUI.fillAmount = _health.GetNormalizedHP();
        _health.OnNormalizedCurrentHPChangedCallback += OnHPChange;
    }

    private void OnDestroy()
    {
        _health.OnNormalizedCurrentHPChangedCallback -= OnHPChange;
    }

    private void OnHPChange(float normalizedHP)
    {
        _healthUI.fillAmount = normalizedHP;
    }
}