using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private Dictionary<ItemModel, int> inventoryItems = new Dictionary<ItemModel, int>();
    [SerializeField] private int maxWeight = 100;
    private int currentWeight = 0;

    
    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler OnInventoryChanged;

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

    void Start()
    {
        UpdateWeight();
        Debug.Log("Inventory started empty.");
    }

    public bool AddItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        
        int newWeight = currentWeight + (item.Weight * quantity);
        if (newWeight > maxWeight)
        {
            Debug.Log($"Can't add item - weight exceeds maximum ({newWeight} > {maxWeight})");
            return false;
        }

        
        if (inventoryItems.ContainsKey(item))
            inventoryItems[item] += quantity;
        else
            inventoryItems.Add(item, quantity);

       
        UpdateWeight();
        OnInventoryChanged?.Invoke();

        return true;
    }

    public void RemoveItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return;

        if (inventoryItems.ContainsKey(item) && inventoryItems[item] >= quantity)
        {
            inventoryItems[item] -= quantity;
            if (inventoryItems[item] <= 0)
                inventoryItems.Remove(item);

            UpdateWeight();
            OnInventoryChanged?.Invoke();
        }
    }

    private void UpdateWeight()
    {
        currentWeight = inventoryItems.Sum(pair => pair.Key.Weight * pair.Value);
    }

    public Dictionary<ItemModel, int> GetInventoryItems()
    {
        return new Dictionary<ItemModel, int>(inventoryItems);
    }

    public int GetCurrentWeight()
    {
        return currentWeight;
    }

    public int GetMaxWeight()
    {
        return maxWeight;
    }

    public bool IsAtWeightLimit()
    {
        
        return currentWeight >= maxWeight * 0.9f;
    }

    public int GetItemQuantity(ItemModel item)
    {
        if (inventoryItems.ContainsKey(item))
            return inventoryItems[item];
        return 0;
    }

    public bool HasItem(ItemModel item, int quantity)
    {
        if (inventoryItems.ContainsKey(item))
            return inventoryItems[item] >= quantity;
        return false;
    }
}