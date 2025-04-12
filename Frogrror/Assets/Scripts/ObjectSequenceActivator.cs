using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSequenceActivator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectSequence;
    [SerializeField] private float _delay = 0.5f;

    private IEnumerator Start()
    {
        for (var i = 1; i < _objectSequence.Count; i++)
        {
            yield return new WaitForSeconds(_delay);
            _objectSequence[i - 1].SetActive(false);
            _objectSequence[i].SetActive(true);
        }
    }
}
