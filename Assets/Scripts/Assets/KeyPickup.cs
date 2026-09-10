using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string requiredKeyID = "RedDoor";
    public Animator doorAnimator;
    public string isOpenParam = "IsOpen";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasKey(requiredKeyID))
        {
            doorAnimator.SetBool(isOpenParam, true);
            Debug.Log("ใช้กุญแจ " + requiredKeyID + " เปิดประตูสำเร็จ");
        }
        else
        {
            Debug.Log("ต้องการกุญแจ: " + requiredKeyID);
        }
    }
}
