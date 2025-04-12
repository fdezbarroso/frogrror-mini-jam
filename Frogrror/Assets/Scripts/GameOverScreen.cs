using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private RectTransform _buttonsContainer;
    [SerializeField] private float _fadeDuration = 0.5f;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(1.5f);
        sequence.Append(_canvasGroup.DOFade(1f, _fadeDuration));
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() =>
        {
            _buttonsContainer.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
        });
    }
}
