using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMPro.TMP_Text quantityText;
    private ItemModel item;

    public void Setup(ItemModel itemModel, int quantity)
    {
        item = itemModel;
        iconImage.sprite = item.Icon;
        quantityText.text = $"x{quantity}";
    }

    public ItemModel GetItem()
    {
        return item;
    }
}