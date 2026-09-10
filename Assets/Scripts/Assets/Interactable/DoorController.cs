using UnityEngine;
using TMPro; // ถ้าใช้ TextMeshPro สำหรับข้อความแจ้งเตือน

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = false;
    public string requiredKeyID = "RedKey";

    [Header("References")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;

    private Animator animator;
    private bool isOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (isLocked)
        {
            PlayerInventory inventory = PlayerInventory.Instance; // ใช้ Singleton แทน GetComponent

            if (inventory != null && inventory.HasKey(requiredKeyID))
            {
                isLocked = false;
                Debug.Log("ปลดล็อกประตูสำเร็จ!");
                ToggleDoor();
            }
            else
            {
                Debug.Log("ประตูล็อกอยู่ ต้องมี: " + requiredKeyID);
                if (promptText != null)
                    promptText.text = "Requires " + requiredKeyID;
            }
        }
        else
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (promptUI != null)
            {
                promptUI.SetActive(true);
                if (promptText != null)
                    promptText.text = isLocked ? "Locked" : "[E] Open/Close";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
