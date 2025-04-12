using System;
using DG.Tweening;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.25f;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup =  GetComponent<CanvasGroup>();
    }

    public void Show(Action onShow = null)
    {
        _canvasGroup.blocksRaycasts = true;
        var tween = _canvasGroup.DOFade(1f, _fadeDuration);
        tween.OnComplete(() =>
        {
            onShow?.Invoke();
        });
    }

    public void Hide(Action onHide = null)
    {
        var tween = _canvasGroup.DOFade(0f, _fadeDuration);
        tween.OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = false;
            onHide?.Invoke();
        });
    }
}
