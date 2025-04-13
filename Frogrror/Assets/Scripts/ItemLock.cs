using System;
using UnityEngine;

public class ItemLock : MonoBehaviour
{
    [SerializeField] private string _requiredItemId;
    [SerializeField] private GameObject _lock;
    [SerializeField] private string _noItemMessage;


    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        if (other.gameObject == player.gameObject)
        {
            if (player.GetItemById(_requiredItemId) != null)
            {
                Destroy(_lock);
                Destroy(gameObject);
            }
            else
            {
                GameplayManager.Instance.DialogueUI.ShowMessage(_noItemMessage);
            }
        }
    }
}
