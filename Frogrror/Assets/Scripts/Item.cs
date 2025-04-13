using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite icon;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private string pickUpMessage;
    [SerializeField] private string id;

    public Sprite Icon => icon;

    public string ID => id;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        GameplayManager.Instance.Player.TakeItem(this);
        GameplayManager.Instance.DialogueUI.ShowMessage(pickUpMessage);

        if (_audioManager && pickUpSound)
        {
            _audioManager.PlaySoundEffect(pickUpSound);
        }
    }
}