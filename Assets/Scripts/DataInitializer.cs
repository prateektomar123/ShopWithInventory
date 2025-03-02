using UnityEngine;
using System.Collections.Generic;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private List<ItemModel> allItems = new List<ItemModel>();

    private void Start()
    {
        InitializeItemData();
    }
    public void InitializeItemData()
    {
        // Find all ItemModel scriptable objects
        ItemModel[] items = Resources.FindObjectsOfTypeAll<ItemModel>();
        allItems.Clear();
        allItems.AddRange(items);

        foreach (var item in allItems)
        {
            // Set item descriptions if empty
            if (string.IsNullOrEmpty(item.Itemdescription))
            {
                item.Itemdescription = GenerateItemDescription(item);
            }

            // Set buying and selling prices based on rarity if not set
            if (item.BuyingPrice <= 0)
            {
                item.BuyingPrice = GetDefaultBuyingPrice(item.rarity);
            }

            if (item.sellingPrice <= 0)
            {
                item.sellingPrice = Mathf.RoundToInt(item.BuyingPrice * 0.7f); // Sell for 70% of buy price
            }

            // Set weight based on type and rarity if not set
            if (item.Weight <= 0)
            {
                item.Weight = GetDefaultWeight(item.type, item.rarity);
            }
        }

        Debug.Log($"Initialized data for {allItems.Count} items");
    }

    private string GenerateItemDescription(ItemModel item)
    {
        // Generate a description based on item type and rarity
        string description = "";

        switch (item.type)
        {
            case ItemModel.ItemType.Food:
                description = GetFoodDescription(item.name, item.rarity);
                break;
            case ItemModel.ItemType.Materials:
                description = GetMaterialDescription(item.name, item.rarity);
                break;
            case ItemModel.ItemType.Weapons:
                description = GetWeaponDescription(item.name, item.rarity);
                break;
            case ItemModel.ItemType.Consumables:
                description = GetConsumableDescription(item.name, item.rarity);
                break;
            default:
                description = $"A {item.rarity.ToString().ToLower()} {item.type.ToString().ToLower()}.";
                break;
        }

        return description;
    }

    private string GetFoodDescription(string name, ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return $"A basic food item that provides minimal sustenance.";
            case ItemModel.Rarity.Common:
                return $"A decent meal that will keep you going for a while.";
            case ItemModel.Rarity.Rare:
                return $"A high-quality dish prepared with skill and good ingredients.";
            case ItemModel.Rarity.Epic:
                return $"An exquisite delicacy known to restore vitality and energy.";
            case ItemModel.Rarity.Legendary:
                return $"A mythical food said to grant temporary powers to those who consume it.";
            default:
                return $"A food item.";
        }
    }

    private string GetMaterialDescription(string name, ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return $"A basic crafting material found in abundance.";
            case ItemModel.Rarity.Common:
                return $"A standard crafting material used in many recipes.";
            case ItemModel.Rarity.Rare:
                return $"A valuable material sought after by skilled crafters.";
            case ItemModel.Rarity.Epic:
                return $"An exceptional material that can create powerful items.";
            case ItemModel.Rarity.Legendary:
                return $"A legendary material of incredible power and rarity.";
            default:
                return $"A crafting material.";
        }
    }

    private string GetWeaponDescription(string name, ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return $"A simple weapon of basic construction.";
            case ItemModel.Rarity.Common:
                return $"A standard weapon of decent craftsmanship.";
            case ItemModel.Rarity.Rare:
                return $"A finely crafted weapon made by a skilled weaponsmith.";
            case ItemModel.Rarity.Epic:
                return $"An exceptional weapon with unique properties.";
            case ItemModel.Rarity.Legendary:
                return $"A legendary weapon whose name is known throughout the land.";
            default:
                return $"A weapon.";
        }
    }

    private string GetConsumableDescription(string name, ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return $"A basic potion with minor effects.";
            case ItemModel.Rarity.Common:
                return $"A standard potion with useful effects.";
            case ItemModel.Rarity.Rare:
                return $"A potent potion brewed by a skilled alchemist.";
            case ItemModel.Rarity.Epic:
                return $"An exceptional elixir with powerful and long-lasting effects.";
            case ItemModel.Rarity.Legendary:
                return $"A mythical concoction of extraordinary power.";
            default:
                return $"A consumable item.";
        }
    }

    private int GetDefaultBuyingPrice(ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return Random.Range(5, 15);
            case ItemModel.Rarity.Common:
                return Random.Range(20, 50);
            case ItemModel.Rarity.Rare:
                return Random.Range(75, 200);
            case ItemModel.Rarity.Epic:
                return Random.Range(300, 800);
            case ItemModel.Rarity.Legendary:
                return Random.Range(1000, 5000);
            default:
                return 10;
        }
    }

    private int GetDefaultWeight(ItemModel.ItemType type, ItemModel.Rarity rarity)
    {
        int baseWeight;

        // Base weight by type
        switch (type)
        {
            case ItemModel.ItemType.Food:
                baseWeight = 2;
                break;
            case ItemModel.ItemType.Materials:
                baseWeight = 3;
                break;
            case ItemModel.ItemType.Weapons:
                baseWeight = 8;
                break;
            case ItemModel.ItemType.Consumables:
                baseWeight = 1;
                break;
            default:
                baseWeight = 3;
                break;
        }

        // Adjust by rarity (legendary items are lighter/more valuable per weight)
        float rarityFactor;
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                rarityFactor = 1.2f;
                break;
            case ItemModel.Rarity.Common:
                rarityFactor = 1.0f;
                break;
            case ItemModel.Rarity.Rare:
                rarityFactor = 0.8f;
                break;
            case ItemModel.Rarity.Epic:
                rarityFactor = 0.6f;
                break;
            case ItemModel.Rarity.Legendary:
                rarityFactor = 0.4f;
                break;
            default:
                rarityFactor = 1.0f;
                break;
        }

        return Mathf.Max(1, Mathf.RoundToInt(baseWeight * rarityFactor));
    }
}