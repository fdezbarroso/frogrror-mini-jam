using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.25f;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup =  GetComponent<CanvasGroup>();
    }

    public void ChangeScene(string sceneName)
    {
        _canvasGroup.blocksRaycasts = true;
        var tween = _canvasGroup.DOFade(1f, _fadeDuration);
        tween.OnComplete(() =>
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName);
            if (loadOperation != null)
            {
                loadOperation.completed += OnSceneLoaded;
            }
        });
    }

    private void OnSceneLoaded(AsyncOperation loadOperation)
    {
        var tween = _canvasGroup.DOFade(0f, _fadeDuration);
        tween.OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = false;
        });
    }
}
