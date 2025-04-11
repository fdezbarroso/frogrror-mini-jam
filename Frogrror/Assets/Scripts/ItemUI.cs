using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;

    private Item _item;
    
    public void SetupItem(Item item)
    {
        _item = item;
        _itemIcon.sprite = item.Icon;
    }
}
