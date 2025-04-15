using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        var sceneChanger = GameManager.Instance.SceneChanger;

        if (other.gameObject != player.gameObject) return;
        
        player.enabled = false;
        sceneChanger.ChangeScene(_nextSceneName);
    }
}
