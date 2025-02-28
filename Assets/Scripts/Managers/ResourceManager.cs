using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private Button gatherResourcesButton;
    [SerializeField] private AudioClip gatherSound;
    [SerializeField] private AudioSource audioSource;

    
    [SerializeField] private float gatherCooldown = 5f;
    private float gatherTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        if (gatherResourcesButton != null)
        {
            gatherResourcesButton.onClick.AddListener(GatherResources);
        }
    }

    private void Update()
    {
        
        if (gatherTimer > 0)
        {
            gatherTimer -= Time.deltaTime;
            if (gatherTimer <= 0)
            {
                EnableGatherButton();
            }
        }
    }

    private void GatherResources()
    {
        
        if (InventoryManager.Instance.IsAtWeightLimit())
        {
            UIManager.Instance.ShowNotification("Inventory is too heavy to gather more resources!");
            return;
        }

       
        DisableGatherButton();
        gatherTimer = gatherCooldown;

        
        if (audioSource != null && gatherSound != null)
        {
            audioSource.PlayOneShot(gatherSound);
        }

        
        int inventoryValue = CalculateInventoryValue();

        
        ItemModel.Rarity rarity = DetermineRarity(inventoryValue);

        
        ItemModel randomItem = GetRandomItemOfRarity(rarity);

        if (randomItem != null)
        {
            
            if (InventoryManager.Instance.AddItem(randomItem, 1))
            {
                
                ShowGatherNotification(randomItem, rarity);

                
                UIManager.Instance.UpdateWeightDisplay();
            }
            else
            {
                UIManager.Instance.ShowNotification("Inventory is too full!");
            }
        }
        else
        {
            UIManager.Instance.ShowNotification("Found nothing this time.");
        }
    }

    private int CalculateInventoryValue()
    {
        int totalValue = 0;
        var inventoryItems = InventoryManager.Instance.GetInventoryItems();

        foreach (var pair in inventoryItems)
        {
            totalValue += pair.Key.sellingPrice * pair.Value;
        }

        return totalValue;
    }

    private ItemModel.Rarity DetermineRarity(int inventoryValue)
    {
        
        float legendaryChance = 0.01f;
        float epicChance = 0.05f;
        float rareChance = 0.15f;
        float commonChance = 0.30f;

        
        float inventoryFactor = inventoryValue / 10000f;
        legendaryChance += inventoryFactor * 0.04f;
        epicChance += inventoryFactor * 0.1f;
        rareChance += inventoryFactor * 0.15f;

       
        legendaryChance = Mathf.Min(legendaryChance, 0.1f);
        epicChance = Mathf.Min(epicChance, 0.2f);
        rareChance = Mathf.Min(rareChance, 0.3f);

        
        float roll = Random.value;

        if (roll < legendaryChance)
            return ItemModel.Rarity.Legendary;
        else if (roll < legendaryChance + epicChance)
            return ItemModel.Rarity.Epic;
        else if (roll < legendaryChance + epicChance + rareChance)
            return ItemModel.Rarity.Rare;
        else if (roll < legendaryChance + epicChance + rareChance + commonChance)
            return ItemModel.Rarity.Common;
        else
            return ItemModel.Rarity.VeryCommon;
    }

    private ItemModel GetRandomItemOfRarity(ItemModel.Rarity rarity)
    {
        
        List<ItemModel> itemsOfRarity = new List<ItemModel>();

        
        ItemModel[] allItems = Resources.FindObjectsOfTypeAll<ItemModel>();

       
        foreach (ItemModel item in allItems)
        {
            if (item.rarity == rarity)
            {
                itemsOfRarity.Add(item);
            }
        }

        
        if (itemsOfRarity.Count == 0)
        {
            if (rarity == ItemModel.Rarity.VeryCommon)
                return null;

            ItemModel.Rarity lowerRarity = (ItemModel.Rarity)Mathf.Max(0, (int)rarity - 1);
            return GetRandomItemOfRarity(lowerRarity);
        }

        
        return itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];
    }

    private void ShowGatherNotification(ItemModel item, ItemModel.Rarity rarity)
    {
        string message;

        switch (rarity)
        {
            case ItemModel.Rarity.Legendary:
                message = $"<color=#FFD700>You found a LEGENDARY {item.name}!</color>";
                break;
            case ItemModel.Rarity.Epic:
                message = $"<color=#A335EE>You found an EPIC {item.name}!</color>";
                break;
            case ItemModel.Rarity.Rare:
                message = $"<color=#0070DD>You found a RARE {item.name}!</color>";
                break;
            case ItemModel.Rarity.Common:
                message = $"You found a common {item.name}.";
                break;
            default:
                message = $"You found a {item.name}.";
                break;
        }

        UIManager.Instance.ShowNotification(message);
    }

    private void DisableGatherButton()
    {
        if (gatherResourcesButton != null)
        {
            gatherResourcesButton.interactable = false;
        }
    }

    private void EnableGatherButton()
    {
        if (gatherResourcesButton != null)
        {
            gatherResourcesButton.interactable = true;
        }
    }
}