using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _textDuration = 3.0f;
    
    private Coroutine _coroutine;

    public void ShowMessage(string message)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        _text.text = message;
        yield return new WaitForSeconds(_textDuration);
        _text.text = string.Empty;
    }
}
