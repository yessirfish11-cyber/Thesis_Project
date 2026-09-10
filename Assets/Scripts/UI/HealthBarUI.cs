using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private static HealthBarUI _instance;
    public static HealthBarUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<HealthBarUI>();
            return _instance;
        }
    }

    [Header("อ้างอิง UI")]
    public Slider healthSlider;
    public CanvasGroup canvasGroup; // ใช้คุมการโชว์/ซ่อนแบบเฟด (ใส่ CanvasGroup ที่ Panel ของ Health Bar)

    [Header("การแสดงผลชั่วคราว")]
    public float displayDuration = 3f;  // เวลาที่แสดงค้างไว้หลังโดนตี (วินาที)
    public float fadeSpeed = 5f;        // ความเร็วในการเฟดเข้า-ออก

    private PlayerHealth currentTarget;
    private float hideTimer = 0f;
    private bool isVisible = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (canvasGroup == null) return;

        if (isVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f) isVisible = false;
        }

        float targetAlpha = isVisible ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    // เรียกจาก PlayerSwitcher เมื่อเปลี่ยนตัวละคร
    public void SetTarget(PlayerHealth playerHealth)
    {
        if (currentTarget != null)
        {
            currentTarget.OnHealthChanged -= UpdateBar;
        }

        currentTarget = playerHealth;

        if (currentTarget != null)
        {
            currentTarget.OnHealthChanged += UpdateBar;

            if (healthSlider != null)
            {
                healthSlider.maxValue = currentTarget.maxLives;
                healthSlider.value = currentTarget.CurrentLives;
            }
        }
    }

    void UpdateBar(int current, int max)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = max;
        healthSlider.value = current;

        // โดนตี (หรือเปลี่ยนค่าเลือด) → โชว์ UI และรีเซ็ตเวลานับถอยหลัง
        ShowTemporarily();
    }

    void ShowTemporarily()
    {
        isVisible = true;
        hideTimer = displayDuration;
    }

    void OnDestroy()
    {
        if (currentTarget != null)
        {
            currentTarget.OnHealthChanged -= UpdateBar;
        }
    }
}
