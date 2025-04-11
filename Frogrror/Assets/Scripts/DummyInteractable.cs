using UnityEngine;

public class DummyInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Dummy Interact");
    }
}
