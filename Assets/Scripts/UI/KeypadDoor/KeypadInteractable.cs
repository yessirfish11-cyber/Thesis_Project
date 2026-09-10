using Unity.VisualScripting;
using UnityEngine;

public class KeypadInteractable : MonoBehaviour
{
    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Ref")]
    public KeypadDoor targetDoor;   // ประตูที่จะปลดล็อก
    public KeypadUI keypadUI;       // UI หน้าต่างกดรหัส

    private bool playerInRange = false;
    private bool isUIOpen = false;

    void Update()
    {
        if (isUIOpen)
        {
            // กด Esc เพื่อปิดหน้าต่างระหว่างกดรหัสอยู่
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseKeypad();
            }
            return;
        }

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            OpenKeypad();
        }
    }

    void OpenKeypad()
    {
        if (targetDoor != null && targetDoor.IsUnlocked)
        {
            return; // ประตูปลดล็อกไปแล้ว ไม่ต้องเปิดหน้าต่างอีก
        }

        isUIOpen = true;
        keypadUI.Open(targetDoor);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseKeypad()
    {
        isUIOpen = false;
        keypadUI.Close();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // ถ้าเดินออกจากระยะระหว่างกดรหัสอยู่ ให้ปิดหน้าต่างอัตโนมัติ
            if (isUIOpen)
            {
                CloseKeypad();
            }
        }
    }
}
