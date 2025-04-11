using UnityEngine;

public class HidingPlace : MonoBehaviour, IInteractable
{
    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        var player = GameplayManager.Instance.Player;

        if (player.IsHiding)
        {
            player.Show();
        }
        else
        {
            player.Hide();
        }
    }
}
