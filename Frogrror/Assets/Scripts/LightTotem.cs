using UnityEngine;

public class LightTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractableHandler _interactableHandler;
    [SerializeField] private GameObject _light;
    [SerializeField] private AudioClip _activateSfx;
    
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
        
        GameManager.Instance.AudioManager.PlaySoundEffect(_activateSfx);
    }
}
