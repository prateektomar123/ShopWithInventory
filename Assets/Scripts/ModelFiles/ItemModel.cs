using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ShopInventory/Create New Item")]
public class ItemModel : ScriptableObject
{
    public enum ItemType
    {
        Food,
        Materials,
        Weapons,
        Consumables
    }
    public enum Rarity
    {
        VeryCommon,
        Common,
        Rare,
        Epic,
        Legendary
    }

    public ItemType type;
    public Sprite Icon;
    public string Itemdescription;
    public int BuyingPrice;
    public int sellingPrice;
    public int Weight;
    public int Quantity;
    public Rarity rarity;


}
