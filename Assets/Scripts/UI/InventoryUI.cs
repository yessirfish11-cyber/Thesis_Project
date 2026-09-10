using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot Images")]
    public Image[] slotIcons;      // ลาก Image ของแต่ละช่อง (ตัวแสดงไอคอนไอเทม)
    public GameObject[] slotFrames; // ลาก GameObject กรอบช่อง (ถ้าต้องการ highlight ช่องที่มีของ)

    [Header("Empty Slot Icon")]
    public Sprite emptySlotSprite; // ไม่บังคับ ถ้าอยากให้ช่องว่างมีไอคอนจางๆ

    void OnEnable()
    {
        // รอให้ PlayerInventory พร้อมก่อน แล้วค่อยสมัครฟัง Event
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += RefreshUI;
            RefreshUI(); // อัปเดตทันทีตอนเปิดใช้งาน
        }
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    void RefreshUI()
    {
        var items = PlayerInventory.Instance.GetAllItems();

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < items.Count)
            {
                // ช่องนี้มีของ
                slotIcons[i].sprite = items[i].itemIcon;
                slotIcons[i].enabled = items[i].itemIcon != null;
                slotIcons[i].color = Color.white;
            }
            else
            {
                // ช่องว่าง
                slotIcons[i].sprite = emptySlotSprite;
                slotIcons[i].enabled = emptySlotSprite != null;
            }
        }
    }
}
