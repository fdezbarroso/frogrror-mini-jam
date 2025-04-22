using UnityEngine;

public class LightTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableHandler _interactableHandler;
    [SerializeField] private GameObject _light;
    
    public bool Activated { get; private set; }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        if (Activated)
        {
            return;
        }
        
        Activated = true;
        Destroy(_interactableHandler);
        
        _light.SetActive(true);
    }
}
