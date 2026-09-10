using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crouching : MonoBehaviour
{
    [Header("ขนาดตัวละคร")]
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);

    [Header("อ้างอิงสคริปต์เดิน/วิ่ง")]
    public PlayerMovement playerMovement; // ลาก PlayerMovement component มาใส่ใน Inspector

    public bool IsCrouching { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            IsCrouching = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            IsCrouching = false;
        }
    }
}
