using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;

    public Item item;

    public void SetupItem(Item inItem)
    {
        item = inItem;
        _itemIcon.sprite = item.Icon;
    }
}