using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopModel : MonoBehaviour
{
    public static ShopModel Instance { get; private set; }

    [SerializeField] private List<ItemModel> shopItemList = new List<ItemModel>(); 
    private Dictionary<ItemModel, int> shopItems = new Dictionary<ItemModel, int>(); 

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
        foreach (var item in shopItemList)
        {
            if (!shopItems.ContainsKey(item))
            {
                shopItems.Add(item, 10); 
            }
        }
        Debug.Log($"Shop initialized with {shopItems.Count} items.");
    }

    public List<ItemModel> GetItemsByType(ItemModel.ItemType type)
    {
        return shopItems.Keys.Where(item => item.type == type).ToList();
    }

    public int GetItemQuantity(ItemModel item)
    {
        return shopItems.ContainsKey(item) ? shopItems[item] : 0;
    }

    public bool BuyItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0 || !shopItems.ContainsKey(item)) return false;

        int available = shopItems[item];
        if (available < quantity)
        {
            Debug.Log($"Not enough {item.name} in shop! Available: {available}");
            return false;
        }

        int cost = item.BuyingPrice * quantity;
        if (!CurrencyManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough gold!");
            return false;
        }

        if (InventoryManager.Instance.AddItem(item, quantity))
        {
            shopItems[item] -= quantity;
            CurrencyManager.Instance.RemoveGold(cost);
            Debug.Log($"Bought {quantity}x {item.name} for {cost} gold.");
            return true;
        }

        return false;
    }

    public void AddItem(ItemModel item, int quantity)
    {
        if (shopItems.ContainsKey(item))
        {
            shopItems[item] += quantity;
        }
        else
        {
            shopItems.Add(item, quantity);
        }
    }
}