using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;

    [Header("Item Detail Panel")]
    [SerializeField] private GameObject itemDetailPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI itemWeightText;
    [SerializeField] private TextMeshProUGUI itemRarityText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;

    [Header("Quantity Selection Popup")]
    [SerializeField] private GameObject quantityPopup;
    [SerializeField] private TextMeshProUGUI quantityTitleText;
    [SerializeField] private TextMeshProUGUI quantityValueText;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button quantityConfirmButton;
    [SerializeField] private Button quantityCancelButton;

    [Header("Confirmation Popup")]
    [SerializeField] private GameObject confirmationPopup;
    [SerializeField] private TextMeshProUGUI confirmationMessageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buySoundEffect;
    [SerializeField] private AudioClip sellSoundEffect;
    [SerializeField] private AudioClip errorSoundEffect;

    
    private ItemModel currentItem;
    private bool isShopItem;
    private int selectedQuantity = 1;

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
        
        HideItemDetail();
        HideQuantityPopup();
        HideConfirmationPopup();
        HideNotification();

        
        UpdateCurrencyDisplay();
        UpdateWeightDisplay();
    }

   

    public void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = CurrencyManager.Instance.GetGold().ToString() + " Gold";
        }
    }

    public void UpdateWeightDisplay()
    {
        if (weightText != null)
        {
            int currentWeight = InventoryManager.Instance.GetCurrentWeight();
            int maxWeight = InventoryManager.Instance.GetMaxWeight();
            weightText.text = $"Weight: {currentWeight}/{maxWeight}";
        }
    }

    public void ShowNotification(string message, float duration = 3f)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);

            
            if (notificationCoroutine != null)
                StopCoroutine(notificationCoroutine);

            
            notificationCoroutine = StartCoroutine(HideNotificationAfterDelay(duration));
        }
    }

    private Coroutine notificationCoroutine;

    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideNotification();
        notificationCoroutine = null;
    }

    private void HideNotification()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    

    public void ShowItemDetail(ItemModel item, bool isFromShop)
    {
        if (item == null || itemDetailPanel == null) return;

        currentItem = item;
        isShopItem = isFromShop;

        
        if (itemIcon != null) itemIcon.sprite = item.Icon;
        if (itemNameText != null) itemNameText.text = item.name;
        if (itemDescriptionText != null) itemDescriptionText.text = item.Itemdescription;
        if (itemWeightText != null) itemWeightText.text = $"Weight: {item.Weight}";
        if (itemRarityText != null) itemRarityText.text = $"Rarity: {item.rarity}";

       
        if (isFromShop)
        {
            if (itemPriceText != null) itemPriceText.text = $"Buy Price: {item.BuyingPrice} Gold";
            if (actionButtonText != null) actionButtonText.text = "Buy";

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(InitiateBuyItem);
            }
        }
        else
        {
            if (itemPriceText != null) itemPriceText.text = $"Sell Price: {item.sellingPrice} Gold";
            if (actionButtonText != null) actionButtonText.text = "Sell";

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(InitiateSellItem);
            }
        }

        
        itemDetailPanel.SetActive(true);
    }

    public void HideItemDetail()
    {
        if (itemDetailPanel != null)
            itemDetailPanel.SetActive(false);

        currentItem = null;
    }

    private void InitiateBuyItem()
    {
        if (currentItem == null || !isShopItem) return;

        
        if (!CurrencyManager.Instance.CanAfford(currentItem.BuyingPrice))
        {
            ShowNotification("Not enough gold!");
            PlaySound(errorSoundEffect);
            return;
        }

        
        int currentWeight = InventoryManager.Instance.GetCurrentWeight();
        int maxWeight = InventoryManager.Instance.GetMaxWeight();

        if (currentWeight + currentItem.Weight > maxWeight)
        {
            ShowNotification("Inventory too heavy!");
            PlaySound(errorSoundEffect);
            return;
        }

       
        ShowQuantityPopup(true);
    }

    private void InitiateSellItem()
    {
        if (currentItem == null || isShopItem) return;

        
        int quantity = InventoryManager.Instance.GetItemQuantity(currentItem);
        if (quantity <= 0)
        {
            ShowNotification("You don't have any of this item!");
            PlaySound(errorSoundEffect);
            return;
        }

       
        ShowQuantityPopup(false);
    }

   

    private void ShowQuantityPopup(bool isBuying)
    {
        if (quantityPopup == null || currentItem == null) return;

        
        selectedQuantity = 1;

       
        if (quantityTitleText != null)
            quantityTitleText.text = isBuying ? "Buy Quantity" : "Sell Quantity";

        
        if (quantityValueText != null)
            quantityValueText.text = selectedQuantity.ToString();

        
        if (decreaseButton != null)
        {
            decreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(() => {
                selectedQuantity = Mathf.Max(1, selectedQuantity - 1);
                quantityValueText.text = selectedQuantity.ToString();
            });
        }

       
        if (increaseButton != null)
        {
            increaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.AddListener(() => {
                int maxQuantity = isBuying ?
                    CalculateMaxBuyQuantity() :
                    InventoryManager.Instance.GetItemQuantity(currentItem);

                selectedQuantity = Mathf.Min(maxQuantity, selectedQuantity + 1);
                quantityValueText.text = selectedQuantity.ToString();
            });
        }

        
        if (quantityConfirmButton != null)
        {
            quantityConfirmButton.onClick.RemoveAllListeners();
            quantityConfirmButton.onClick.AddListener(() => {
                HideQuantityPopup();
                ShowConfirmationPopup(isBuying);
            });
        }

       
        if (quantityCancelButton != null)
        {
            quantityCancelButton.onClick.RemoveAllListeners();
            quantityCancelButton.onClick.AddListener(HideQuantityPopup);
        }

        
        quantityPopup.SetActive(true);
    }

    private void HideQuantityPopup()
    {
        if (quantityPopup != null)
            quantityPopup.SetActive(false);
    }

    private int CalculateMaxBuyQuantity()
    {
        if (currentItem == null) return 1;

        
        int maxAffordable = CurrencyManager.Instance.GetGold() / currentItem.BuyingPrice;

        
        int remainingWeight = InventoryManager.Instance.GetMaxWeight() - InventoryManager.Instance.GetCurrentWeight();
        int maxByWeight = Mathf.Max(1, remainingWeight / currentItem.Weight);

        
        int maxAvailable = ShopModel.Instance.GetItemQuantity(currentItem);

        
        return Mathf.Max(1, Mathf.Min(Mathf.Min(maxAffordable, maxByWeight), maxAvailable));
    }

   

    private void ShowConfirmationPopup(bool isBuying)
    {
        if (confirmationPopup == null || currentItem == null) return;

       
        int totalPrice = isBuying ?
            currentItem.BuyingPrice * selectedQuantity :
            currentItem.sellingPrice * selectedQuantity;

        
        if (confirmationMessageText != null)
        {
            confirmationMessageText.text = isBuying ?
                $"Buy {selectedQuantity}x {currentItem.name} for {totalPrice} gold?" :
                $"Sell {selectedQuantity}x {currentItem.name} for {totalPrice} gold?";
        }

        
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => {
                HideConfirmationPopup();

                if (isBuying)
                    CompletePurchase();
                else
                    CompleteSale();
            });
        }

        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(HideConfirmationPopup);
        }

      
        confirmationPopup.SetActive(true);
    }

    private void HideConfirmationPopup()
    {
        if (confirmationPopup != null)
            confirmationPopup.SetActive(false);
    }

    private void CompletePurchase()
    {
        if (currentItem == null) return;

        
        int totalCost = currentItem.BuyingPrice * selectedQuantity;

        
        if (ShopModel.Instance.BuyItem(currentItem, selectedQuantity))
        {
            
            PlaySound(buySoundEffect);
            ShowNotification($"You bought {selectedQuantity}x {currentItem.name}");

            
            UpdateCurrencyDisplay();
            UpdateWeightDisplay();

           
            HideItemDetail();
        }
        else
        {
            ShowNotification("Failed to purchase items.");
            PlaySound(errorSoundEffect);
        }
    }

    private void CompleteSale()
    {
        if (currentItem == null) return;

       
        if (ShopModel.Instance.SellItem(currentItem, selectedQuantity))
        {
            
            int totalValue = currentItem.sellingPrice * selectedQuantity;

            
            PlaySound(sellSoundEffect);
            ShowNotification($"You gained {totalValue} gold");

           
            UpdateCurrencyDisplay();
            UpdateWeightDisplay();

            
            HideItemDetail();
        }
        else
        {
            ShowNotification("Failed to sell items.");
            PlaySound(errorSoundEffect);
        }
    }

   

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}