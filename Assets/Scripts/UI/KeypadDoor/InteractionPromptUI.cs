using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public enum PromptType { Item, Door }

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

    [Header("Item Prompt UI")]
    public GameObject itemPromptPanel;
    public TMP_Text itemPromptText;

    [Header("Door Prompt UI")]
    public GameObject doorPromptPanel;
    public TMP_Text doorPromptText;

    private object currentItemSource;
    private object currentDoorSource;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (itemPromptPanel != null) itemPromptPanel.SetActive(false);
        if (doorPromptPanel != null) doorPromptPanel.SetActive(false);
    }

    public void Show(string message, object source, PromptType type)
    {
        if (type == PromptType.Item)
        {
            currentItemSource = source;
            if (itemPromptPanel != null) itemPromptPanel.SetActive(true);
            if (itemPromptText != null) itemPromptText.text = message;
        }
        else if (type == PromptType.Door)
        {
            currentDoorSource = source;
            if (doorPromptPanel != null) doorPromptPanel.SetActive(true);
            if (doorPromptText != null) doorPromptText.text = message;
        }
    }

    public void Hide(object source, PromptType type)
    {
        if (type == PromptType.Item)
        {
            if (currentItemSource != null && currentItemSource != source) return;
            currentItemSource = null;
            if (itemPromptPanel != null) itemPromptPanel.SetActive(false);
        }
        else if (type == PromptType.Door)
        {
            if (currentDoorSource != null && currentDoorSource != source) return;
            currentDoorSource = null;
            if (doorPromptPanel != null) doorPromptPanel.SetActive(false);
        }
    }
}
