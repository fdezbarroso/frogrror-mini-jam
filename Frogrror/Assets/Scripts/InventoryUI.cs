using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private ItemUI _itemUIPrefab;

    private List<ItemUI> _itemUIs = new List<ItemUI>();
    
    public void OnItemAdded(Item item)
    {
        var itemUIInstance = Instantiate(_itemUIPrefab, _container);
        itemUIInstance.SetupItem(item);

        _itemUIs.Add(itemUIInstance);
    }

    public void OnItemRemoved(Item item)
    {
        ItemUI itemUIInstance = _itemUIs.Find(x => x.item == item);
        _itemUIs.Remove(itemUIInstance);
        Destroy(itemUIInstance.gameObject);
    }
}