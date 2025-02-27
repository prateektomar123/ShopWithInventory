using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    [SerializeField] private Dictionary<ItemModel, int> inventoryItems = new Dictionary<ItemModel, int>();
    [SerializeField] private int MaxWeight = 100;
    private int CurrentWeight = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start() { UpdateWeight(); Debug.Log("Inventory started empty."); }

    public bool AddItem(ItemModel item, int quantity)
    {
        int newWeight = CurrentWeight + (item.Weight * quantity);
        if (newWeight > MaxWeight) return false;

        int currentCount = inventoryItems.Values.Sum();
        if (currentCount + quantity > 16) return false;

        if (inventoryItems.ContainsKey(item))
            inventoryItems[item] += quantity;
        else
            inventoryItems.Add(item, quantity);

        UpdateWeight();
        return true;
    }

    public void RemoveItem(ItemModel item, int quantity)
    {
        if (inventoryItems.ContainsKey(item) && inventoryItems[item] >= quantity)
        {
            inventoryItems[item] -= quantity;
            if (inventoryItems[item] <= 0) inventoryItems.Remove(item);
            UpdateWeight();
        }
    }

    private void UpdateWeight()
    {
        CurrentWeight = inventoryItems.Sum(pair => pair.Key.Weight * pair.Value);
    }

    public Dictionary<ItemModel, int> GetInventoryItems()
    {
        return new Dictionary<ItemModel, int>(inventoryItems);
    }

    public int GetCurrentWeight()
    {
        return CurrentWeight;
    }
}