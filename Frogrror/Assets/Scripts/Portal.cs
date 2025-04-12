using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform _targetPosition;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        var sceneChanger = GameplayManager.Instance.SceneChanger;

        if (other.gameObject != player.gameObject) return;
        
        player.enabled = false;
        sceneChanger.Show(() =>
        {
            player.Teleport(_targetPosition.position);
            sceneChanger.Hide(() =>
            {
                player.enabled = true;
            });
        });
    }
}
