using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private Button[] tabButtons; // 4 tab buttons (Food, Materials, Weapons, Potions)
    [SerializeField] private Color activeTabColor = Color.white;
    [SerializeField] private Color inactiveTabColor = new Color(0.8f, 0.8f, 0.8f);

    private ItemModel.ItemType[] tabTypes = new ItemModel.ItemType[]
    {
        ItemModel.ItemType.Food,
        ItemModel.ItemType.Materials,
        ItemModel.ItemType.Weapons,
        ItemModel.ItemType.Consumables
    };

    void Start()
    {
        inventoryView.InitializePanels();

        
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(tabIndex));
        }

        SelectTab(0); 
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabButtons.Length) return;

        ItemModel.ItemType selectedType = tabTypes[tabIndex];
        inventoryView.ShowPanel(selectedType);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            ColorBlock colors = tabButtons[i].colors;
            colors.normalColor = (i == tabIndex) ? activeTabColor : inactiveTabColor;
            tabButtons[i].colors = colors;
        }

        Debug.Log($"Switched to {selectedType} tab in inventory.");
    }

    
    public void RefreshInventory()
    {
        inventoryView.UpdateAllPanels();
    }
}