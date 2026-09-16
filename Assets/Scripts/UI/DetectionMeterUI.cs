using UnityEngine;
using UnityEngine.UI;

public class DetectionMeterUI : MonoBehaviour
{
    private static DetectionMeterUI _instance;
    public static DetectionMeterUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<DetectionMeterUI>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject panel;
    public Image eyeFillImage; // Image Type = Filled

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (panel != null) panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void UpdateFill(float value)
    {
        if (eyeFillImage != null)
        {
            eyeFillImage.fillAmount = value;
        }
    }
}
