using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image backgroundImage;

    [Header("Rarity Colors")]
    [SerializeField] private Color veryCommonColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color commonColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color rareColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color epicColor = new Color(0.6f, 0.2f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.8f, 0.2f);

    private ItemModel item;
    private int quantity;
    private bool isShopItem;

    public void Setup(ItemModel itemModel, int itemQuantity, bool isFromShop = false)
    {
        item = itemModel;
        quantity = itemQuantity;
        isShopItem = isFromShop;

        if (iconImage != null)
            iconImage.sprite = item.Icon;

        if (quantityText != null)
            quantityText.text = $"x{quantity}";

       
        if (backgroundImage != null)
        {
            Color rarityColor = GetRarityColor(item.rarity);
            backgroundImage.color = rarityColor;
        }
    }

    private Color GetRarityColor(ItemModel.Rarity rarity)
    {
        switch (rarity)
        {
            case ItemModel.Rarity.VeryCommon:
                return veryCommonColor;
            case ItemModel.Rarity.Common:
                return commonColor;
            case ItemModel.Rarity.Rare:
                return rareColor;
            case ItemModel.Rarity.Epic:
                return epicColor;
            case ItemModel.Rarity.Legendary:
                return legendaryColor;
            default:
                return commonColor;
        }
    }

    public ItemModel GetItem()
    {
        return item;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public bool IsShopItem()
    {
        return isShopItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && UIManager.Instance != null)
        {
            
            UIManager.Instance.ShowItemDetail(item, isShopItem);
        }
    }
}