using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory _instance;
    public static PlayerInventory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<PlayerInventory>();
            return _instance;
        }
    }

    private List<InventoryItemData> items = new List<InventoryItemData>();

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void AddItem(ItemPickup item)
    {
        items.Add(new InventoryItemData(item)); // เก็บเป็นข้อมูล ไม่ใช่ reference ของ component
        Debug.Log("Inventory ตอนนี้มี: " + items.Count + " ชิ้น");
        OnInventoryChanged?.Invoke();
    }

    public bool HasKey(string requiredKeyID)
    {
        foreach (var item in items)
        {
            bool isKeyType = item.itemType == ItemPickup.ItemType.Key
                           || item.itemType == ItemPickup.ItemType.KeyCard;
            if (isKeyType && item.keyID == requiredKeyID)
                return true;
        }
        return false;
    }

    public bool UseKey(string requiredKeyID)
    {
        InventoryItemData found = items.Find(i =>
            (i.itemType == ItemPickup.ItemType.Key || i.itemType == ItemPickup.ItemType.KeyCard)
            && i.keyID == requiredKeyID);

        if (found != null)
        {
            items.Remove(found);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public List<InventoryItemData> GetAllItems()
    {
        return items;
    }

    public bool HasItemOfType(ItemPickup.ItemType type)
    {
        foreach (var item in items)
        {
            if (item.itemType == type) return true;
        }
        return false;
    }

    public void RemoveItem(InventoryItemData item)
    {
        if (items.Remove(item))
        {
            OnInventoryChanged?.Invoke();
        }
    }
}
