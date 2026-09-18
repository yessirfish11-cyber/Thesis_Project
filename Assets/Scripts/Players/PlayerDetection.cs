using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Vision Increase Rate / Vision Buff")]
    public float increaseRatePerSecond = 0.5f;
    public float decreaseRatePerSecond = 0.25f;

    public float DetectionValue { get; private set; }
    public bool IsFullyDetected => DetectionValue >= 1f;
    public bool IsEmpty => DetectionValue <= 0f;

    private bool seenThisFrame = false;

    public void ReportSeen()
    {
        seenThisFrame = true;
    }

    void LateUpdate()
    {
        float previousValue = DetectionValue;

        if (seenThisFrame)
        {
            DetectionValue += increaseRatePerSecond * Time.deltaTime;
        }
        else
        {
            DetectionValue -= decreaseRatePerSecond * Time.deltaTime;
        }

        DetectionValue = Mathf.Clamp01(DetectionValue);
        Debug.Log(gameObject.name + " LateUpdate ทำงาน | seenThisFrame: " + seenThisFrame + " | ค่าก่อน: " + previousValue.ToString("F3") + " | ค่าหลัง: " + DetectionValue.ToString("F3"));

        if (DetectionValue > 0f && previousValue <= 0f)
        {
            DetectionMeterUI.Instance?.Show();
        }
        else if (DetectionValue <= 0f && previousValue > 0f)
        {
            DetectionMeterUI.Instance?.Hide();
        }

        if (DetectionValue > 0f)
        {
            DetectionMeterUI.Instance?.UpdateFill(DetectionValue);
        }

        seenThisFrame = false;
    }

    public void ResetDetectionInstant()
    {
        DetectionValue = 0f;
        seenThisFrame = false;
        DetectionMeterUI.Instance?.Hide();
    }
}
