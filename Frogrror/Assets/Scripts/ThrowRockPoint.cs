using UnityEngine;

public class ThrowRockPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactMessageNoRock;
    
    [SerializeField] private Transform _startingPosition;
    [SerializeField] private float _throwForce = 100f;
    [SerializeField] private Vector2 _throwDirection = Vector2.up;
    [SerializeField] private AudioClip _throwSound;
    
    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        var player = GameplayManager.Instance.Player;
        var item = player.GetItemById("Rock");

        if (item == null)
        {
            
            GameplayManager.Instance.DialogueUI.ShowMessage(_interactMessageNoRock);
            return;
        }
        
        player.RemoveItemById("Rock");
        
        item.transform.position = _startingPosition.position;

        item.GetComponent<FloatingMovement>().enabled = false;
        
        item.gameObject.SetActive(true);
        
        GameManager.Instance.AudioManager.PlaySoundEffect(_throwSound);
        
        var itemRigidbody = item.GetComponent<Rigidbody2D>();
        itemRigidbody.gravityScale = 1f;
        itemRigidbody.AddForce(_throwDirection * _throwForce);

        Destroy(item, 10f);

        Destroy(gameObject);
    }
}
