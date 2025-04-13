using UnityEngine;

public class ThrowRockPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _startingPosition;
    [SerializeField] private float _throwForce = 100f;
    [SerializeField] private Vector2 _throwDirection = Vector2.up;
    
    
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
            return;
        }
        
        player.RemoveItemById("Rock");
        
        item.transform.position = _startingPosition.position;

        item.GetComponent<FloatingMovement>().enabled = false;
        
        item.gameObject.SetActive(true);
        
        var itemRigidbody = item.GetComponent<Rigidbody2D>();
        itemRigidbody.gravityScale = 1f;
        itemRigidbody.AddForce(_throwDirection * _throwForce);

        Destroy(item, 10f);
    }
}
