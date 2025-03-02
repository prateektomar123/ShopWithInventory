using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryView : MonoBehaviour
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
    private void Start()
    {
        InitializePanels();
    }

    public void InitializePanels()
    {
        UpdatePanel(ItemModel.ItemType.Food);
        UpdatePanel(ItemModel.ItemType.Materials);
        UpdatePanel(ItemModel.ItemType.Weapons);
        UpdatePanel(ItemModel.ItemType.Consumables);

        ShowPanel(ItemModel.ItemType.Food);


    }


    public void UpdatePanel(ItemModel.ItemType type)
    {
        GridLayoutGroup panel = typeToPanel[type];
        foreach (Transform child in panel.transform) Destroy(child.gameObject);

        var allItems = InventoryManager.Instance.GetInventoryItems();
        foreach (var pair in allItems)
        {
            if (pair.Key.type == type)
            {
                GameObject slot = Instantiate(itemSlotPrefab, panel.transform);
                slot.GetComponent<ItemSlot>().Setup(pair.Key, pair.Value,false);
            }
        }

        Debug.Log("INVENTORY: " + type);
    }


    public void ShowPanel(ItemModel.ItemType type)
    {
        foreach (var panel in typeToPanel.Values)
        {
            panel.gameObject.SetActive(false);
        }
        typeToPanel[type].gameObject.SetActive(true);
    }

   
    public void UpdateAllPanels()
    {
        UpdatePanel(ItemModel.ItemType.Food);
        UpdatePanel(ItemModel.ItemType.Materials);
        UpdatePanel(ItemModel.ItemType.Weapons);
        UpdatePanel(ItemModel.ItemType.Consumables);
    }
}