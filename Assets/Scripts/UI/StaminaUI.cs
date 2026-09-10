using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaUI : MonoBehaviour
{
    private static StaminaUI _instance;
    public static StaminaUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<StaminaUI>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject staminaPanel;
    public Slider staminaSlider;
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float delayBeforeFade = 1f;
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

    public void UpdateStamina(float percent01)
    {
        if (staminaSlider != null)
            staminaSlider.value = percent01;
    }

    public void ShowImmediate()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (staminaPanel != null) staminaPanel.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

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
        if (staminaPanel != null) staminaPanel.SetActive(false);
        hideRoutine = null;
    }
}
