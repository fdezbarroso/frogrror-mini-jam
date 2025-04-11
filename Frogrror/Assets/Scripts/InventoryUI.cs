using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private ItemUI _itemUIPrefab;

    private List<ItemUI> _itemUIs = new List<ItemUI>();
    
    private void Start()
    {
        GameplayManager.Instance.Player.OnItemAdded += OnItemAdded;
    }

    private void OnItemAdded(Item item)
    {
        var itemUIInstance = Instantiate(_itemUIPrefab, _container);
        itemUIInstance.SetupItem(item);
        
        _itemUIs.Add(itemUIInstance);
    }
}
