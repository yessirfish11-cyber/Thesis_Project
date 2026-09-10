using UnityEngine;
using System.Collections.Generic;

public class PlayerSwitcher : MonoBehaviour
{
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
            SetPlayerActive(i, false);
        }

        int firstValidIndex = FindFirstValidPlayer();
        if (firstValidIndex != -1)
        {
            ActivatePlayer(firstValidIndex);
        }
        else
        {
            Debug.LogError("❌ ไม่พบ Player ที่สมบูรณ์ในรายการ! โปรดตรวจสอบใน Inspector");
        }
    }

    // ✅ เพิ่มฟังก์ชันใหม่: ค้นหา index แรกที่ไม่เป็น Null
    int FindFirstValidPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null &&
                players[i].playerObject != null &&
                players[i].playerCamera != null)
            {
                return i;
            }
        }
        return -1; // ไม่เจอเลย
    }

    void ActivatePlayer(int index)
    {
        if (index < 0 || index >= players.Count) return;

        var slot = players[index];
        if (slot == null || slot.playerObject == null || slot.playerCamera == null)
        {
            Debug.LogWarning($"⚠️ Slot ที่ {index} ข้อมูลไม่ครบ ข้ามไป...");
            return;
        }

        currentPlayerIndex = index;
        SetPlayerActive(index, true);

        // PlayerHealth.OnEnable() จะลงทะเบียนกับ Health Bar เองอัตโนมัติแล้ว
        // แต่ยังต้องผูก Death Event ไว้ตรงนี้ เพื่อรู้ว่าเมื่อไหร่ต้องสลับตัว
        PlayerHealth health = slot.playerObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
            health.OnPlayerDeath += HandlePlayerDeath;
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
        // ยกเลิกการฟัง Event ของตัวเก่า
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
            }
            SetPlayerActive(currentPlayerIndex, false);
        }

        // ✅ ปรับ: หาตัวถัดไปที่ใช้งานได้จริง (ข้ามตัวที่หาย/Null)
        int nextIndex = currentPlayerIndex;
        do
        {
            nextIndex++;
            if (nextIndex >= players.Count)
            {
                Debug.Log("✅ ALL PLAYERS ELIMINATED - Game Over");
                return;
            }
        }
        while (players[nextIndex] == null ||
               players[nextIndex].playerObject == null ||
               players[nextIndex].playerCamera == null);

        // เปิดตัวถัดไปที่ใช้ได้จริง
        ActivatePlayer(nextIndex);
    }
}
