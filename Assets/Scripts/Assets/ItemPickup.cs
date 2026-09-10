using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        Generic,    // ไอเทมทั่วไป (เก็บไว้เฉยๆ ในกระเป๋า)
        Key,        // กุญแจ ต้องมี KeyID ระบุว่าใช้เปิดประตูไหน
        KeyCard,    // การ์ดผ่านประตู
        Document,   // เอกสาร/บันทึก (อ่านได้)
        Battery,    // ไอเทมใช้แล้วหมด เช่น ถ่านไฟฉาย
        Tool,       // เครื่องมือ ใช้ปลดล็อกกลไกบางอย่าง
        Torch

    }

    [Header("Base Item Data")]
    public string itemName = "Item";
    public ItemType itemType = ItemType.Generic;
    public Sprite itemIcon; // ไอคอนแสดงใน Inventory UI (ถ้ามี)

    [Header("Type-Specific Data")]
    public string keyID = "";        // ใช้กับ Key/KeyCard เช่น "RedDoor", "Room_003"
    [TextArea(2, 5)]
    public string documentText = ""; // ใช้กับ Document

    [Header("Interact")]
    public KeyCode pickupKey = KeyCode.E;

    [Header("In-Container Condition")]
    public DrawerInteractable parentDrawer; // ถ้าของอยู่ในลิ้นชัก ต้องเปิดก่อนถึงหยิบได้ (ปล่อยว่างได้ถ้าวางเปิดโล่ง)

    private bool playerInRange = false;
    private bool wasShowingPrompt = false;

    void Awake()
    {
        // ตั้งค่า Rigidbody ให้เป็น Kinematic อัตโนมัติ ไม่ต้องตั้งเองทุกครั้ง
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        bool canPickup = parentDrawer == null || parentDrawer.IsOpen;

        if (playerInRange && canPickup)
        {
            if (!wasShowingPrompt) // แสดงแค่ครั้งแรกที่เข้าเงื่อนไข ไม่ใช่ทุกเฟรม
            {
                InteractionPromptUI.Instance?.Show($"[E] {itemName}", this);
                wasShowingPrompt = true;
            }
        }
        else
        {
            if (wasShowingPrompt)
            {
                InteractionPromptUI.Instance?.Hide(this);
                wasShowingPrompt = false;
            }
        }

        if (!canPickup) return;
        if (playerInRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        PlayerInventory inventory = PlayerInventory.Instance; // เปลี่ยนจาก FindFirstObjectByType
        if (inventory != null) inventory.AddItem(this);
        InteractionPromptUI.Instance?.Hide(this);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter บน " + gameObject.name + " ชนกับ: " + other.name);
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionPromptUI.Instance?.Hide(this);
        }
    }
}
