using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private ShopModel shopModel;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Button gatherResourcesButton;

    [Header("Popups")]
    [SerializeField] private GameObject quantityPopup;
    [SerializeField] private GameObject confirmationPopup;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buySoundEffect;
    [SerializeField] private AudioClip sellSoundEffect;
    [SerializeField] private AudioClip errorSoundEffect;

    [Header("ItemDetailPanel")]
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI DetailweightText;
    [SerializeField] TextMeshProUGUI rarityText;
    [SerializeField] Button actionButton;
    [SerializeField] TextMeshProUGUI actionButtonText;

    [Header("ConfirmationPopup")]
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton; 


    [Header("Quantity_Popup")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] Button decreaseButton;
    [SerializeField] Button increaseButton;
    [SerializeField] Button QuantityconfirmButton;
    [SerializeField] Button QuantitycancelButton;


    private ItemModel selectedItem;
    private bool isShopItem; 
    private int selectedQuantity = 1;

    
    private float notificationTimer = 0f;
    private float notificationDuration = 3f;

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
        
        itemDetailPanel.SetActive(false);
        quantityPopup.SetActive(false);
        confirmationPopup.SetActive(false);
        notificationText.gameObject.SetActive(false);

        
        gatherResourcesButton.onClick.AddListener(GatherResources);

        
        UpdateCurrencyDisplay();
        UpdateWeightDisplay();
    }

    private void Update()
    {
        
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0)
            {
                notificationText.gameObject.SetActive(false);
            }
        }
    }

    

    public void UpdateCurrencyDisplay()
    {
        currencyText.text = currencyManager.GetGold().ToString() + " Gold";
    }

    public void UpdateWeightDisplay()
    {
        int currentWeight = inventoryManager.GetCurrentWeight();
        int maxWeight = inventoryManager.GetMaxWeight();
        weightText.text = $"Weight: {currentWeight}/{maxWeight}";
    }

    public void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        notificationTimer = notificationDuration;
    }

   

    public void SelectShopItem(ItemModel item)
    {
        selectedItem = item;
        isShopItem = true;
        UpdateItemDetailPanel();
    }

    public void SelectInventoryItem(ItemModel item)
    {
        selectedItem = item;
        isShopItem = false;
        UpdateItemDetailPanel();
    }

    private void UpdateItemDetailPanel()
    {
        if (selectedItem == null)
        {
            itemDetailPanel.SetActive(false);
            return;
        }

        iconImage.sprite = selectedItem.Icon;
        nameText.text = selectedItem.name;
        descriptionText.text = selectedItem.Itemdescription;
        DetailweightText.text = $"Weight: {selectedItem.Weight}";
        rarityText.text = $"Rarity: {selectedItem.rarity}";

       
        if (isShopItem)
        {
            priceText.text = $"Buy Price: {selectedItem.BuyingPrice} Gold";
            actionButtonText.text = "Buy";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(InitiateBuy);
        }
        else
        {
            priceText.text = $"Sell Price: {selectedItem.sellingPrice} Gold";
            actionButtonText.text = "Sell";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(InitiateSell);
        }

        
        itemDetailPanel.SetActive(true);
    }

    

    private void InitiateBuy()
    {
        if (selectedItem == null || !isShopItem) return;

       
        if (!currencyManager.CanAfford(selectedItem.BuyingPrice))
        {
            ShowNotification("Not enough gold!");
            PlaySound(errorSoundEffect);
            return;
        }

        
        if (inventoryManager.GetCurrentWeight() + selectedItem.Weight > inventoryManager.GetMaxWeight())
        {
            ShowNotification("Inventory too heavy!");
            PlaySound(errorSoundEffect);
            return;
        }

        
        ShowQuantityPopup(true);
    }

    private void InitiateSell()
    {
        if (selectedItem == null || isShopItem) return;

       
        ShowQuantityPopup(false);
    }

    private void ShowQuantityPopup(bool isBuying)
    {
        
        selectedQuantity = 1;
        quantityText.text = selectedQuantity.ToString();

       
        titleText.text = isBuying ? "Buy Quantity" : "Sell Quantity";

        decreaseButton.onClick.RemoveAllListeners();
        decreaseButton.onClick.AddListener(() => {
            selectedQuantity = Mathf.Max(1, selectedQuantity - 1);
            quantityText.text = selectedQuantity.ToString();
        });

        increaseButton.onClick.RemoveAllListeners();
        increaseButton.onClick.AddListener(() => {
            int maxQuantity = isBuying ?
                CalculateMaxBuyQuantity() :
                inventoryManager.GetItemQuantity(selectedItem);

            selectedQuantity = Mathf.Min(maxQuantity, selectedQuantity + 1);
            quantityText.text = selectedQuantity.ToString();
        });

        QuantityconfirmButton.onClick.RemoveAllListeners();
        QuantityconfirmButton.onClick.AddListener(() => {
            quantityPopup.SetActive(false);
            ShowConfirmationPopup(isBuying);
        });

        QuantitycancelButton.onClick.RemoveAllListeners();
        QuantitycancelButton.onClick.AddListener(() => {
            quantityPopup.SetActive(false);
        });

       
        quantityPopup.SetActive(true);
    }

    private int CalculateMaxBuyQuantity()
    {
        
        int maxAffordable = currencyManager.GetGold() / selectedItem.BuyingPrice;

        
        int remainingWeight = inventoryManager.GetMaxWeight() - inventoryManager.GetCurrentWeight();
        int maxByWeight = remainingWeight / selectedItem.Weight;

        
        return Mathf.Max(1, Mathf.Min(maxAffordable, maxByWeight));
    }

    private void ShowConfirmationPopup(bool isBuying)
    {
        
        

        
        int totalPrice = isBuying ?
            selectedItem.BuyingPrice * selectedQuantity :
            selectedItem.sellingPrice * selectedQuantity;

       
        messageText.text = isBuying ?
            $"Buy {selectedQuantity}x {selectedItem.name} for {totalPrice} gold?" :
            $"Sell {selectedQuantity}x {selectedItem.name} for {totalPrice} gold?";

        
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => {
            confirmationPopup.SetActive(false);

            if (isBuying)
                CompletePurchase();
            else
                CompleteSale();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => {
            confirmationPopup.SetActive(false);
        });

        
        confirmationPopup.SetActive(true);
    }

    private void CompletePurchase()
    {
        
        int totalCost = selectedItem.BuyingPrice * selectedQuantity;

       
        if (!currencyManager.CanAfford(totalCost))
        {
            ShowNotification("Not enough gold!");
            PlaySound(errorSoundEffect);
            return;
        }

        
        if (inventoryManager.AddItem(selectedItem, selectedQuantity))
        {
           
            currencyManager.RemoveGold(totalCost);

           
            shopModel.RemoveItem(selectedItem, selectedQuantity);

            
            PlaySound(buySoundEffect);
            ShowNotification($"You bought {selectedQuantity}x {selectedItem.name}");

            
            UpdateCurrencyDisplay();
            UpdateWeightDisplay();

            
            FindObjectOfType<InventoryController>().RefreshInventory();

            
            itemDetailPanel.SetActive(false);
        }
        else
        {
            ShowNotification("Cannot carry more items!");
            PlaySound(errorSoundEffect);
        }
    }

    private void CompleteSale()
    {
        
        int totalValue = selectedItem.sellingPrice * selectedQuantity;

       
        inventoryManager.RemoveItem(selectedItem, selectedQuantity);

        
        currencyManager.AddGold(totalValue);

        
        shopModel.AddItem(selectedItem, selectedQuantity);

        
        PlaySound(sellSoundEffect);
        ShowNotification($"You gained {totalValue} gold");

        
        UpdateCurrencyDisplay();
        UpdateWeightDisplay();

        
        FindObjectOfType<InventoryController>().RefreshInventory();

        
        itemDetailPanel.SetActive(false);
    }

    

    private void GatherResources()
    {
        
        if (inventoryManager.IsAtWeightLimit())
        {
            ShowNotification("Inventory is too heavy to gather more resources!");
            PlaySound(errorSoundEffect);
            return;
        }

        
        ItemModel[] veryCommonItems = GetItemsByRarity(ItemModel.Rarity.VeryCommon);
        ItemModel[] commonItems = GetItemsByRarity(ItemModel.Rarity.Common);
        ItemModel[] rareItems = GetItemsByRarity(ItemModel.Rarity.Rare);
        ItemModel[] epicItems = GetItemsByRarity(ItemModel.Rarity.Epic);
        ItemModel[] legendaryItems = GetItemsByRarity(ItemModel.Rarity.Legendary);

        
        int inventoryValue = CalculateInventoryValue();

       
        float rareChance = Mathf.Min(0.3f, 0.05f + (inventoryValue / 10000f));
        float epicChance = Mathf.Min(0.15f, 0.01f + (inventoryValue / 20000f));
        float legendaryChance = Mathf.Min(0.05f, 0.005f + (inventoryValue / 50000f));

        
        float roll = Random.value;
        ItemModel randomItem = null;

        if (roll < legendaryChance && legendaryItems.Length > 0)
        {
            randomItem = legendaryItems[Random.Range(0, legendaryItems.Length)];
            ShowNotification("Found a LEGENDARY item!");
        }
        else if (roll < epicChance && epicItems.Length > 0)
        {
            randomItem = epicItems[Random.Range(0, epicItems.Length)];
            ShowNotification("Found an EPIC item!");
        }
        else if (roll < rareChance && rareItems.Length > 0)
        {
            randomItem = rareItems[Random.Range(0, rareItems.Length)];
            ShowNotification("Found a RARE item!");
        }
        else if (Random.value < 0.6f && commonItems.Length > 0)
        {
            randomItem = commonItems[Random.Range(0, commonItems.Length)];
            ShowNotification("Found a common item.");
        }
        else if (veryCommonItems.Length > 0)
        {
            randomItem = veryCommonItems[Random.Range(0, veryCommonItems.Length)];
            ShowNotification("Found a very common item.");
        }

        
        if (randomItem != null)
        {
            if (inventoryManager.AddItem(randomItem, 1))
            {
                
                PlaySound(buySoundEffect);

                
                UpdateWeightDisplay();

               
                FindObjectOfType<InventoryController>().RefreshInventory();
            }
            else
            {
                ShowNotification("Inventory is too full!");
                PlaySound(errorSoundEffect);
            }
        }
        else
        {
            ShowNotification("Found nothing this time.");
        }
    }

    private ItemModel[] GetItemsByRarity(ItemModel.Rarity rarity)
    {
        
        ItemModel[] allItems = Resources.FindObjectsOfTypeAll<ItemModel>();

       
        return System.Array.FindAll(allItems, item => item.rarity == rarity);
    }

    private int CalculateInventoryValue()
    {
        int totalValue = 0;
        var inventoryItems = inventoryManager.GetInventoryItems();

        foreach (var pair in inventoryItems)
        {
            totalValue += pair.Key.sellingPrice * pair.Value;
        }

        return totalValue;
    }

   

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}