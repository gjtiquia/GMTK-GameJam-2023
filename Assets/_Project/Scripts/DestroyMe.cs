using System.Collections;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    [SerializeField] private float _destroyDelay;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(this.gameObject);
    }
}