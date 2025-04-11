using UnityEngine;

public class DummyInteractable : MonoBehaviour, IInteractable
{
    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        Debug.Log($"Interacting with {GetName()}");
    }
}
