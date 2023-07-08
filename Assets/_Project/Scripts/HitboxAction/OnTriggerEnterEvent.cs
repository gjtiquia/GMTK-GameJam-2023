using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onTriggerEnter?.Invoke();
    }
}