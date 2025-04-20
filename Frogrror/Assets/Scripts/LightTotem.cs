using UnityEngine;

public class LightTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableHandler _interactableHandler;
    [SerializeField] private GameObject _light;
    
    private bool _activated;

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        if (_activated)
        {
            return;
        }
        
        _activated = true;
        Destroy(_interactableHandler);
        
        _light.SetActive(true);
    }
}
