using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    private static InteractionPromptUI _instance;
    public static InteractionPromptUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<InteractionPromptUI>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    // เก็บ "เจ้าของ" ข้อความที่กำลังแสดงอยู่ตอนนี้
    private object currentSource;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    // ต้องระบุ source (ปกติคือ this ของ ItemPickup ที่เรียก)
    public void Show(string message, object source)
    {
        currentSource = source;
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptText != null) promptText.text = message;
    }

    // Hide จะทำงานก็ต่อเมื่อคนที่เรียกคือเจ้าของข้อความปัจจุบันเท่านั้น
    public void Hide(object source)
    {
        if (currentSource != null && currentSource != source) return; // ไม่ใช่เจ้าของ ห้ามเคลียร์ทับ
        currentSource = null;
        if (promptPanel != null) promptPanel.SetActive(false);
    }
}
