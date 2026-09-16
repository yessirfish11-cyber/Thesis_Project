using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Animator")]
    public Animator doorAnimator;
    public string isOpenParam = "IsOpen";

    [Header("Hidden")]
    public Transform hidePoint;
    public Transform exitPoint;

    [Header("Layer HIdden")]
    public string hiddenLayerName = "Hidden";

    private bool playerInRange = false;
    private bool isPlayerHiding = false;

    private GameObject currentPlayer;
    private PlayerTeleporter teleporter;
    private MonoBehaviour playerMovementScript;
    private MouseLook playerMouseLook;
    public Transform facingDirection;
    private int originalLayer;
    private Vector3 positionBeforeHiding;
    private Quaternion rotationBeforeHiding;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!isPlayerHiding) EnterHiding();
            else ExitHiding();
        }

        // Failsafe: ล็อกตำแหน่งไว้ทุกเฟรมระหว่างซ่อนตัว
        if (isPlayerHiding && currentPlayer != null && hidePoint != null)
        {
            currentPlayer.transform.position = hidePoint.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlayerHiding) return;
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerHiding)
        {
            playerInRange = false;
            currentPlayer = null;
        }
    }

    void EnterHiding()
    {
        if (currentPlayer == null || hidePoint == null) return;

        isPlayerHiding = true;
        positionBeforeHiding = currentPlayer.transform.position;
        rotationBeforeHiding = currentPlayer.transform.rotation;

        PlayerDetection detection = currentPlayer.GetComponent<PlayerDetection>();
        if (detection != null)
        {
            detection.ResetDetectionInstant(); // ล้างค่าดวงตาทันที -> Killer จะเลิกไล่ทันทีด้วย เพราะ IsEmpty = true
        }

        Quaternion enterRot = facingDirection != null ? facingDirection.rotation : hidePoint.rotation;

        teleporter = currentPlayer.GetComponent<PlayerTeleporter>();
        if (teleporter != null)
        {
            teleporter.TeleportTo(hidePoint.position, enterRot);
            teleporter.FreezeController();
        }

        playerMovementScript = currentPlayer.GetComponent<PlayerMovement>();
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        playerMouseLook = currentPlayer.GetComponentInChildren<MouseLook>();
        if (playerMouseLook != null)
        {
            playerMouseLook.SyncYRotation(enterRot.eulerAngles.y);
            playerMouseLook.enabled = false;
        }

        originalLayer = currentPlayer.layer;
        int hiddenLayer = LayerMask.NameToLayer(hiddenLayerName);
        if (hiddenLayer != -1) currentPlayer.layer = hiddenLayer;

        if (doorAnimator != null) doorAnimator.SetBool(isOpenParam, true);
    }

    void ExitHiding()
    {
        if (currentPlayer == null) return;

        isPlayerHiding = false;

        Vector3 targetExitPos = exitPoint != null ? exitPoint.position : positionBeforeHiding;
        Quaternion targetExitRot = exitPoint != null ? exitPoint.rotation : rotationBeforeHiding;

        if (teleporter != null)
        {
            teleporter.UnfreezeController();
            teleporter.TeleportTo(targetExitPos, targetExitRot);
        }

        currentPlayer.layer = originalLayer;

        if (playerMovementScript != null) playerMovementScript.enabled = true;

        if (playerMouseLook != null)
        {
            playerMouseLook.SyncYRotation(targetExitRot.eulerAngles.y);
            playerMouseLook.enabled = true;
        }

        if (doorAnimator != null) doorAnimator.SetBool(isOpenParam, false);

        playerInRange = false;
        currentPlayer = null;
    }
}