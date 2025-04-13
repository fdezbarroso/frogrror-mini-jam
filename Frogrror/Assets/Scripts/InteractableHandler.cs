using System;
using UnityEngine;

public interface IInteractable
{
    string GetName();
    void Interact();
}

public class InteractableHandler : MonoBehaviour
{
    [SerializeField] private GameObject _instructions;
    
    private IInteractable _interactable;

    private void Awake()
    {
        _interactable = GetComponent<IInteractable>();
    }

    public void ShowInstructions()
    {
        _instructions.SetActive(true);
    }

    public void HideInstructions()
    {
        _instructions.SetActive(false);
    }

    public void Interact()
    {
        HideInstructions();
        _interactable.Interact();
    }

    private void OnDestroy()
    {
        Destroy(_instructions);
    }
}