using UnityEngine;

public interface IInteractable
{
    string GetName();
    void Interact();
}

public class InteractableHandler : MonoBehaviour
{
    private IInteractable _interactable;

    private void Awake()
    {
        _interactable = GetComponent<IInteractable>();
    }

    public void ShowInstructions()
    {
        Debug.LogError($"Press SPACE to interact with {_interactable.GetName()}");
    }

    public void HideInstructions()
    {
        
    }

    public void Interact()
    {
        HideInstructions();
        _interactable.Interact();
    }
}