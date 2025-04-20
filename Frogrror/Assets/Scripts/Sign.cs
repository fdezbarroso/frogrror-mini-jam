using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{
    [SerializeField] private string _message;

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        GameplayManager.Instance.DialogueUI.ShowMessage(_message);
    }
}
