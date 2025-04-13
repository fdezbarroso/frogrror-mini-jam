using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject _killedText;
    [SerializeField] private GameObject _gameFinishedText;
    
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private RectTransform _buttonsContainer;
    [SerializeField] private float _fadeDuration = 0.5f;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(bool killed = true)
    {
        var text = killed ? _killedText : _gameFinishedText;
        text.SetActive(true);
        
        var button = killed ? _retryButton : _continueButton;
        
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        
        button.gameObject.SetActive(true);

        var delay = killed ? 1.5f : 3f;
        
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(delay);
        sequence.Append(_canvasGroup.DOFade(1f, _fadeDuration));
        sequence.AppendInterval(1f);
        sequence.AppendCallback(() =>
        {
            _buttonsContainer.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
        });
    }
}
