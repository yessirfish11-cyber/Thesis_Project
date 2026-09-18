using UnityEngine;
using System.Collections.Generic;

public class PlayerSwitcher : MonoBehaviour
{
    public static event System.Action OnPlayerSwitched;

    [System.Serializable]
    public class PlayerSlot
    {
        public GameObject playerObject;
        public GameObject playerCamera;
    }

    [Header("Player List (Cycle Order)")]
    public List<PlayerSlot> players = new List<PlayerSlot>();

    [Header("UI")]
    public HealthBarUI healthBarUI;

    private int currentPlayerIndex = 0;

    void Start()
    {
        for (int i = 0; i < players.Count; i++)
        {
            SetPlayerActive(i, false); // ตอนเริ่มเกม ยังไม่มีใครตาย ปิดทั้งหมดปกติ
        }

        int firstValidIndex = FindFirstValidPlayer();
        if (firstValidIndex != -1)
        {
            ActivatePlayer(firstValidIndex);
        }
        else
        {
            Debug.LogError("ไม่พบ Player ที่สมบูรณ์ในรายการ!");
        }
    }

    int FindFirstValidPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null && players[i].playerObject != null && players[i].playerCamera != null)
                return i;
        }
        return -1;
    }

    void ActivatePlayer(int index)
    {
        if (index < 0 || index >= players.Count) return;

        var slot = players[index];
        if (slot == null || slot.playerObject == null || slot.playerCamera == null)
        {
            Debug.LogWarning($"Slot ที่ {index} ข้อมูลไม่ครบ ข้ามไป...");
            return;
        }

        currentPlayerIndex = index;
        SetPlayerActive(index, true);

        // เพิ่มตรงนี้ - ตั้ง Tag/Layer กลับให้ถูกต้อง เผื่อโดนเปลี่ยนไปตอนเป็นศพในรอบก่อนหน้า
        RestorePlayerTagAndLayer(slot.playerObject);

        PlayerHealth health = slot.playerObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
            health.OnPlayerDeath += HandlePlayerDeath;
        }

        PlayerDetection detection = slot.playerObject.GetComponent<PlayerDetection>();
        if (detection != null)
        {
            detection.ResetDetectionInstant();
        }
    }

    void RestorePlayerTagAndLayer(GameObject obj)
    {
        obj.tag = "Player";

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
        {
            SetLayerRecursively(obj, playerLayer);
        }
        else
        {
            Debug.LogWarning("ไม่พบ Layer 'Player' กรุณาตรวจสอบการตั้งค่า Layer");
        }
    }

    void SetPlayerActive(int index, bool isActive)
    {
        if (index < 0 || index >= players.Count) return;

        var slot = players[index];
        if (slot == null) return;

        if (slot.playerObject != null)
            slot.playerObject.SetActive(isActive);

        if (slot.playerCamera != null)
            slot.playerCamera.SetActive(isActive);
    }

    void HandlePlayerDeath()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            var oldSlot = players[currentPlayerIndex];
            if (oldSlot != null && oldSlot.playerObject != null)
            {
                PlayerHealth oldHealth = oldSlot.playerObject.GetComponent<PlayerHealth>();
                if (oldHealth != null)
                {
                    oldHealth.OnPlayerDeath -= HandlePlayerDeath;
                }

                // ไม่ SetActive(false) ทั้งตัว - แค่ปิดการควบคุม เหลือศพให้เห็น
                TurnIntoCorpse(oldSlot);
            }
        }

        OnPlayerSwitched?.Invoke();

        int nextIndex = currentPlayerIndex;
        do
        {
            nextIndex++;
            if (nextIndex >= players.Count)
            {
                Debug.Log("ALL PLAYERS ELIMINATED - Game Over");
                return;
            }
        }
        while (players[nextIndex] == null || players[nextIndex].playerObject == null || players[nextIndex].playerCamera == null);

        ActivatePlayer(nextIndex);
    }

    void TurnIntoCorpse(PlayerSlot slot)
    {
        GameObject obj = slot.playerObject;

        if (slot.playerCamera != null)
            slot.playerCamera.SetActive(false);

        PlayerMovement movement = obj.GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        MouseLook mouseLook = obj.GetComponentInChildren<MouseLook>();
        if (mouseLook != null) mouseLook.enabled = false;

        PlayerInteraction interaction = obj.GetComponent<PlayerInteraction>();
        if (interaction != null) interaction.enabled = false;

        CharacterController controller = obj.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        // เพิ่มตรงนี้ - ปิด Animator ก่อน แล้วค่อยรีเซ็ต Rotation ให้ตั้งตรงปกติ
        Animator animator = obj.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // รีเซ็ต Rotation ให้ตั้งตรง ไม่ให้ค้างท่าเอียงจากตอนถูกตี
        obj.transform.rotation = Quaternion.Euler(0f, obj.transform.eulerAngles.y, 0f);

        int defaultLayer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(obj, defaultLayer);
        obj.tag = "Untagged";
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
