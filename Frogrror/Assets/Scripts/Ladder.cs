using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        if (other.gameObject == player.gameObject)
        {
            player.AllowVerticalMovement(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        if (other.gameObject == player.gameObject)
        {
            player.AllowVerticalMovement(false);
        }
    }
}
