using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopModel : MonoBehaviour
{
    public static ShopModel Instance { get; private set; }

    [SerializeField] private List<ItemModel> shopItemList = new List<ItemModel>();
    private Dictionary<ItemModel, int> shopItems = new Dictionary<ItemModel, int>();

    public delegate void ShopChangedHandler();
    public event ShopChangedHandler OnShopChanged;

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
        
        InitializeShop();
    }

    private void InitializeShop()
    {
        
        if (shopItemList.Count == 0)
        {
            
            ItemModel[] allItems = Resources.FindObjectsOfTypeAll<ItemModel>();
            shopItemList.AddRange(allItems);
        }

        
        foreach (var item in shopItemList)
        {
            if (!shopItems.ContainsKey(item))
            {
               
                int initialQuantity = GetInitialQuantityByRarity(item.rarity);
                shopItems.Add(item, initialQuantity);
            }
        }

        Debug.Log($"Shop initialized with {shopItems.Count} items.");
    }

    private int GetInitialQuantityByRarity(ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return 20;
            case ItemModel.Rarity.Common:
                return 15;
            case ItemModel.Rarity.Rare:
                return 10;
            case ItemModel.Rarity.Epic:
                return 5;
            case ItemModel.Rarity.Legendary:
                return 2;
            default:
                return 10;
        }
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
            if (shopItems[item] <= 0)
                shopItems.Remove(item);

            CurrencyManager.Instance.RemoveGold(cost);
            OnShopChanged?.Invoke();

            Debug.Log($"Bought {quantity}x {item.name} for {cost} gold.");
            return true;
        }

        return false;
    }

    public bool SellItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        if (!InventoryManager.Instance.HasItem(item, quantity))
        {
            Debug.Log($"Not enough {item.name} in inventory to sell!");
            return false;
        }

        int value = item.sellingPrice * quantity;

        
        InventoryManager.Instance.RemoveItem(item, quantity);

       
        AddItem(item, quantity);

        
        CurrencyManager.Instance.AddGold(value);

        Debug.Log($"Sold {quantity}x {item.name} for {value} gold.");
        return true;
    }

    public void AddItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return;

        if (shopItems.ContainsKey(item))
        {
            shopItems[item] += quantity;
        }
        else
        {
            shopItems.Add(item, quantity);
        }

        OnShopChanged?.Invoke();
    }

    public void RemoveItem(ItemModel item, int quantity)
    {
        if (item == null || quantity <= 0) return;

        if (shopItems.ContainsKey(item) && shopItems[item] >= quantity)
        {
            shopItems[item] -= quantity;
            if (shopItems[item] <= 0)
                shopItems.Remove(item);

            OnShopChanged?.Invoke();
        }
    }
}