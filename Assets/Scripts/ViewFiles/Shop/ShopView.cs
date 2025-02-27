using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopView : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefab; 
    [SerializeField] private GridLayoutGroup foodPanel;
    [SerializeField] private GridLayoutGroup materialsPanel;
    [SerializeField] private GridLayoutGroup weaponsPanel;
    [SerializeField] private GridLayoutGroup potionsPanel;

    private Dictionary<ItemModel.ItemType, GridLayoutGroup> typeToPanel;

    void Awake()
    {
        typeToPanel = new Dictionary<ItemModel.ItemType, GridLayoutGroup>
        {
            { ItemModel.ItemType.Food, foodPanel },
            { ItemModel.ItemType.Materials, materialsPanel },
            { ItemModel.ItemType.Weapons, weaponsPanel },
            { ItemModel.ItemType.Consumables, potionsPanel }
        };
    }

    public void InitializePanels()
    {
        SpawnItemsInPanel(ItemModel.ItemType.Food);
        SpawnItemsInPanel(ItemModel.ItemType.Materials);
        SpawnItemsInPanel(ItemModel.ItemType.Weapons);
        SpawnItemsInPanel(ItemModel.ItemType.Consumables);

        ShowPanel(ItemModel.ItemType.Food); 
    }

    private void SpawnItemsInPanel(ItemModel.ItemType type)
    {
        GridLayoutGroup panel = typeToPanel[type];
        List<ItemModel> items = ShopModel.Instance.GetItemsByType(type);

        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, panel.transform);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.Setup(item, ShopModel.Instance.GetItemQuantity(item));
        }

        Debug.Log($"Spawned {items.Count} items in {type} panel.");
    }

    public void ShowPanel(ItemModel.ItemType type)
    {
        foreach (var panel in typeToPanel.Values)
        {
            panel.gameObject.SetActive(false);
        }
        typeToPanel[type].gameObject.SetActive(true);
    }
}