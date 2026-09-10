using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerTeleporter : MonoBehaviour
{
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // ย้ายตำแหน่งผู้เล่นอย่างปลอดภัย ไม่ให้โดนแรงโน้มถ่วง/ฟิสิกส์รบกวน
    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        controller.enabled = true;
    }

    // ปิด Controller ไว้เฉยๆ (ใช้ตอนต้องการ "แช่" ตำแหน่งไว้นานๆ เช่นตอนซ่อนตัว)
    public void FreezeController()
    {
        controller.enabled = false;
    }

    public void UnfreezeController()
    {
        controller.enabled = true;
    }
}
