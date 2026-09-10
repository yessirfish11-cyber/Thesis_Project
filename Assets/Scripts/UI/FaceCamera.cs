using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // บังคับหมุนตามกล้องเสมอ ไม่สนใจการหมุนของ Parent (DoorPivot)
        transform.rotation = mainCam.transform.rotation;
    }
}
