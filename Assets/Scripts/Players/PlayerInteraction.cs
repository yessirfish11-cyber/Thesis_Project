using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Camera Radar")]
    public Camera playerCamera;

    [Header("Range Put Item")]
    public float pickupRange = 3f;

    [Header("RIght_Hand")]
    public Transform rightHandPoint;

    private Interactable currentHeldItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentHeldItem == null)
                TryPickup();
            else
                DropCurrentItem();
        }
    }

    void TryPickup()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, ~0, QueryTriggerInteraction.Collide))
        {
            Interactable item = hit.collider.GetComponent<Interactable>();
            if (item != null && !item.isHeld)
            {
                item.OnPickup(rightHandPoint);
                currentHeldItem = item;
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 1f);
    }

    void DropCurrentItem()
    {
        currentHeldItem.OnDrop(rightHandPoint);
        currentHeldItem = null;
    }
}
