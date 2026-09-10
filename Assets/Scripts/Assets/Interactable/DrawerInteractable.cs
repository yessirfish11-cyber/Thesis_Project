using UnityEngine;

public class DrawerInteractable : MonoBehaviour
{
    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Animator Parameter")]
    public string isOpenParam = "IsOpen"; // ชื่อ Bool Parameter ใน Animator

    private Animator animator;
    private bool isOpen = false;
    private bool playerInRange = false;

    public bool IsOpen => isOpen;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            ToggleDrawer();
        }
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;
        animator.SetBool(isOpenParam, isOpen);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
