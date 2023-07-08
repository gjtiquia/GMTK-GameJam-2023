using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum BossAction
{
    NONE = -1,

    SuperAnticipation,
    SuperAttack,

    BasicAnticipation,
    BasicAttack,
}

// (+++) Super Anticipation + Super Attack + Ready to Evade
// (+) Basic Anticipation + Basic Attack

// (----) Super Attack with no Anticipation
// (---) Mixed Anticipation + Attack
// (--) Super Anticipation + Super Attack but not ready to evade 
// (--) Basic Attack with no Anticipation

public class PlayerSatisfaction : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private int _initialplayerSatisfaction;
    [SerializeField] private int _maxPlayerSatisfaction;
    [SerializeField] private float _popupDuration;

    [Header("Bonus Settings")]
    [SerializeField] private int _superPairBonus;

    [Header("Penalty Settings")]
    [SerializeField] private int _noSuperAnticipationPenalty;
    [SerializeField] private int _tooQuickSuperAttackPenalty;


    [Header("References")]
    [SerializeField] private Hero _hero;
    [SerializeField] private Image _satisfactionBar;
    [SerializeField] private GameObject _penaltyUI;
    [SerializeField] private TextMeshProUGUI _penaltyText;
    [SerializeField] private GameObject _bonusUI;
    [SerializeField] private TextMeshProUGUI _bonusText;

    private int _currentPlayerSatisfaction
    {
        get => m_currentPlayerSatisfaction;
        set
        {
            m_currentPlayerSatisfaction = Mathf.Clamp(value, 0, _maxPlayerSatisfaction);
            UpdateUI();
        }
    }
    private int m_currentPlayerSatisfaction;

    private int _superAnticipationCount = 0;
    private int _superAttackCount = 0;

    private Stack<BossAction> _actionStack;

    private Tween _popupTween;

    private void Awake()
    {
        _actionStack = new Stack<BossAction>();
        _actionStack.Push(BossAction.NONE);

        _currentPlayerSatisfaction = _initialplayerSatisfaction;

        ResetAllPopups();
    }

    public void OnSuperAnticipation()
    {
        _superAnticipationCount++;
        _actionStack.Push(BossAction.SuperAnticipation);
    }

    public void OnSuperAttack()
    {
        _superAttackCount++;

        BossAction previousAction = _actionStack.Peek();

        if (previousAction != BossAction.SuperAnticipation)
        {
            _actionStack.Push(BossAction.SuperAttack);
            _currentPlayerSatisfaction -= _noSuperAnticipationPenalty;

            ShowPenaltyUI("Bruh");
            return;
        }

        _actionStack.Pop();

        if (_hero.CanDodgeSuperAttack())
        {
            _currentPlayerSatisfaction += _superPairBonus;
            ShowBonusUI("Dodged Super Attack");
        }
        else
        {
            _currentPlayerSatisfaction -= _tooQuickSuperAttackPenalty;
            ShowPenaltyUI("Anticipation Too Short");
        }
    }

    private void UpdateUI()
    {
        float normalizedSatisfaction = (float)_currentPlayerSatisfaction / _maxPlayerSatisfaction;
        _satisfactionBar.fillAmount = normalizedSatisfaction;
    }

    private void ShowPenaltyUI(string text)
    {
        ResetAllPopups();

        _penaltyText.text = text;
        _penaltyUI.SetActive(true);

        _popupTween = DOVirtual.DelayedCall(_popupDuration, () => _penaltyUI.SetActive(false));
    }

    private void ShowBonusUI(string text)
    {
        ResetAllPopups();

        _bonusText.text = text;
        _bonusUI.SetActive(true);

        _popupTween = DOVirtual.DelayedCall(_popupDuration, () => _bonusUI.SetActive(false));
    }

    private void ResetAllPopups()
    {
        _penaltyUI.SetActive(false);
        _bonusUI.SetActive(false);

        if (_popupTween != null && _popupTween.IsPlaying())
            _popupTween.Kill();
    }
}