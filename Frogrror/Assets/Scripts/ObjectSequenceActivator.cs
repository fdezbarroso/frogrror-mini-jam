using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSequenceActivator : MonoBehaviour
{
    [SerializeField] private string playerWakeUpDialogue;
    [SerializeField] private List<GameObject> objectSequence;
    [SerializeField] private float delay = 0.5f;

    private IEnumerator Start()
    {
        for (var i = 1; i < objectSequence.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            objectSequence[i - 1].SetActive(false);
            objectSequence[i].SetActive(true);
        }

        GameplayManager.Instance.DialogueUI.ShowMessage(playerWakeUpDialogue);
    }
}