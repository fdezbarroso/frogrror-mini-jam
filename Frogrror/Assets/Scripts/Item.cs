using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite _icon;
    
    public Sprite Icon => _icon;
    
    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        GameplayManager.Instance.Player.TakeItem(this);
    }
}
