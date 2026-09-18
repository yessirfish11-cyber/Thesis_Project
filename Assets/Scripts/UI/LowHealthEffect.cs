using UnityEngine;
using UnityEngine.UI;

public class LowHealthEffect : MonoBehaviour
{
    private static LowHealthEffect _instance;
    public static LowHealthEffect Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<LowHealthEffect>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public Image vignetteImage; // Image สีแดงเต็มจอ (Vignette Sprite หรือสี่เหลี่ยมทึบก็ได้)

    [Header("ตั้งค่าการกระพริบ")]
    public float pulseSpeed = 2f;      // ความเร็วในการกระพริบ
    public float minAlpha = 0.1f;      // ความจางที่สุด
    public float maxAlpha = 0.5f;      // ความเข้มที่สุด

    private bool isPulsing = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (vignetteImage != null)
        {
            SetAlpha(0f);
        }
    }

    void Update()
    {
        if (!isPulsing || vignetteImage == null) return;

        // ใช้ Sin wave ทำให้ Alpha แกว่งขึ้นลงแบบกระพริบนุ่มนวล
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // แปลงจาก -1..1 เป็น 0..1
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        SetAlpha(alpha);
    }

    public void StartPulse()
    {
        isPulsing = true;
    }

    public void StopPulse()
    {
        isPulsing = false;
        SetAlpha(0f); // คืนค่ากลับปกติทันที
    }

    void SetAlpha(float alpha)
    {
        if (vignetteImage == null) return;
        Color c = vignetteImage.color;
        c.a = alpha;
        vignetteImage.color = c;
    }
}
