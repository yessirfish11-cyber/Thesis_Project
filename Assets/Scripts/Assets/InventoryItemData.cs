using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public ItemPickup.ItemType itemType;
    public UnityEngine.Sprite itemIcon;
    public string keyID;

    public InventoryItemData(ItemPickup source)
    {
        itemName = source.itemName;
        itemType = source.itemType;
        itemIcon = source.itemIcon;
        keyID = source.keyID;
    }
}
