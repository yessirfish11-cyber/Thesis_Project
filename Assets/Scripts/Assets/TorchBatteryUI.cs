using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TorchBatteryUI : MonoBehaviour
{
    private static TorchBatteryUI _instance;
    public static TorchBatteryUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<TorchBatteryUI>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject batteryPanel;
    public Slider batterySlider;
    public TMP_Text batteryText;
    public CanvasGroup canvasGroup; // ลาก CanvasGroup ที่ติดอยู่บน batteryPanel มาใส่

    [Header("Fade Settings")]
    public float delayBeforeFade = 3f;
    public float fadeDuration = 0.5f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void UpdateBattery(float percent01)
    {
        if (batterySlider != null)
            batterySlider.value = percent01;

        if (batteryText != null)
            batteryText.text = Mathf.RoundToInt(percent01 * 100f) + "%";
    }

    // เรียกตอนเปิดไฟฉาย: โชว์ทันที ไม่ fade
    public void ShowImmediate()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (batteryPanel != null) batteryPanel.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    // เรียกตอนปิดไฟฉาย: รอ 3 วิ แล้วค่อย fade หาย
    public void HideDelayed()
    {
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideDelayedRoutine());
    }

    private IEnumerator HideDelayedRoutine()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float t = 0f;
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (batteryPanel != null) batteryPanel.SetActive(false);
        hideRoutine = null;
    }

    // ใช้ตอนยังไม่เก็บไฟฉายเลย ให้ซ่อนทันทีไม่ต้องรอ
    public void HideImmediate()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (batteryPanel != null) batteryPanel.SetActive(false);
    }
}
