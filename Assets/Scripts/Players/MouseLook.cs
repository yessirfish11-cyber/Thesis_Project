using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public float smoothTime = 0.05f;
    public Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float xRotVelocity = 0f;
    private float yRotVelocity = 0f;
    private float targetX = 0f;
    private float targetY = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            targetY -= mouseY;
            targetY = Mathf.Clamp(targetY, -90f, 90f);
            targetX += mouseX;
        }

        xRotation = Mathf.SmoothDamp(xRotation, targetY, ref xRotVelocity, smoothTime);
        yRotation = Mathf.SmoothDamp(yRotation, targetX, ref yRotVelocity, smoothTime);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void SyncYRotation(float newYAngle)
    {
        yRotation = newYAngle;
        targetX = newYAngle;
    }

    // เพิ่มฟังก์ชันนี้เข้ามา
    public void SetFrozen(bool frozen)
    {
        this.enabled = !frozen;
    }
}
