using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<ItemModel> inventoryItems = new List<ItemModel>(); 
    [SerializeField] private int MaxWeight = 100;
    private int CurrentWeight = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateWeight();
        
    }

    public bool AddItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        int newWeight = CurrentWeight + (item.Weight * quantity);

        if (newWeight > MaxWeight)
        {
            Debug.Log("Cannot add item: max weight!");
            return false;
        }

        for (int i = 0; i < quantity; i++)
        {
            inventoryItems.Add(item);
        }

        UpdateWeight();
        
        return true;
    }

    public void RemoveItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return;

        int removed = 0;
        for (int i = inventoryItems.Count - 1; i >= 0 && removed < quantity; i--)
        {
            if (inventoryItems[i] == item)
            {
                inventoryItems.RemoveAt(i);
                removed++;
            }
        }

        UpdateWeight();
        
    }

    private void UpdateWeight()
    {
        CurrentWeight = 0;
        foreach (var item in inventoryItems)
        {
            CurrentWeight += item.Weight;
        }
    }

    public List<ItemModel> GetInventoryItems()
    {
        return new List<ItemModel>(inventoryItems);
    }

    public int GetCurrentWeight()
    {
        return CurrentWeight;
    }
}