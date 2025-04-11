using UnityEngine;

public interface IInteractable
{
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
        Debug.LogError("Press A.");
    }

    public void HideInstructions()
    {
        Debug.LogError("Nothing.");
    }

    public void Interact()
    {
        _interactable.Interact();
    }
}