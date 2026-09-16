using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = false;
    public string requiredKeyID = "RedKey";

    private Animator animator;
    private bool isOpen = false;
    private bool playerInRange = false;
    private bool wasShowingPrompt = false;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        wasShowingPrompt = false;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.Hide(this, InteractionPromptUI.PromptType.Door);
        }
    }

    void Update()
    {
        UpdatePrompt();

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void UpdatePrompt()
    {
        if (playerInRange)
        {
            string message = isLocked ? $"[E] Locked - Requires {requiredKeyID}" : "[E] Open/Close";

            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.Show(message, this, InteractionPromptUI.PromptType.Door);
                wasShowingPrompt = true;
            }
        }
        else
        {
            if (wasShowingPrompt && InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.Hide(this, InteractionPromptUI.PromptType.Door);
                wasShowingPrompt = false;
            }
        }
    }

    void TryInteract()
    {
        if (isLocked)
        {
            PlayerInventory inventory = PlayerInventory.Instance;

            if (inventory != null && inventory.HasKey(requiredKeyID))
            {
                isLocked = false;
                Debug.Log("Door Unlocked!");
                ToggleDoor();
            }
            else
            {
                Debug.Log("Locked. Requires Key : " + requiredKeyID);

                if (InteractionPromptUI.Instance != null)
                {
                    InteractionPromptUI.Instance.Show($"Need.. {requiredKeyID}", this, InteractionPromptUI.PromptType.Door);
                }
            }
        }
        else
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
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
            wasShowingPrompt = false;
        }
    }
}
