using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _pickUpMessage;
    [SerializeField] private string _id;
    
    public Sprite Icon => _icon;
    
    public string ID => _id;
    
    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        GameplayManager.Instance.Player.TakeItem(this);
        GameplayManager.Instance.DialogueUI.ShowMessage(_pickUpMessage);
    }
}
