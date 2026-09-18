using UnityEngine;
using System.Collections.Generic;

public class PowerPanelController : MonoBehaviour
{
    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Animator CutOut")]
    public Animator animator;
    public string isOnParam = "IsOn";

    [Header("Lighting")]
    public List<Light> lightsToControl = new List<Light>();
    public List<GameObject> extraObjectsToEnable = new List<GameObject>();

    public bool IsPowerOn { get; private set; } = false;

    private bool playerInRange = false;
    private bool isInteracting = false;
    private GameObject currentPlayerObject;

    void Awake()
    {
        SetLightsActive(false);
    }

    void OnEnable()
    {
        PlayerSwitcher.OnPlayerSwitched += ResetInteraction;
    }

    void OnDisable()
    {
        PlayerSwitcher.OnPlayerSwitched -= ResetInteraction;
    }

    void ResetInteraction()
    {
        playerInRange = false;
        isInteracting = false;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.Hide(this, InteractionPromptUI.PromptType.Door);
        }
    }

    void Update()
    {
        if (IsPowerOn || isInteracting) return;

        if (playerInRange)
        {
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.Show("[E] Power On", this, InteractionPromptUI.PromptType.Door);
            }

            if (Input.GetKeyDown(interactKey))
            {
                StartQTE();
            }
        }
    }

    void StartQTE()
    {
        isInteracting = true;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.Hide(this, InteractionPromptUI.PromptType.Door);
        }

        if (PowerPanelQTEUI.Instance != null)
        {
            PowerPanelQTEUI.Instance.Open(OnQTEResult, currentPlayerObject);
        }
    }

    void OnQTEResult(bool success)
    {
        isInteracting = false;

        if (success)
        {
            TurnOnPower();
        }
    }

    void TurnOnPower()
    {
        IsPowerOn = true;

        if (animator != null)
        {
            animator.SetBool(isOnParam, true);
        }

        SetLightsActive(true);
    }

    void SetLightsActive(bool active)
    {
        foreach (var light in lightsToControl)
        {
            if (light != null) light.enabled = active;
        }

        foreach (var obj in extraObjectsToEnable)
        {
            if (obj != null) obj.SetActive(active);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayerObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.Hide(this, InteractionPromptUI.PromptType.Door);
            }
        }
    }
}
