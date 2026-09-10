using UnityEngine;

[RequireComponent(typeof(Animator))]
public class KeypadDoor : MonoBehaviour
{
    [Header("Animator Door")]
    public string isOpenParam = "IsOpen";

    private Animator animator;
    public bool IsUnlocked { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Unlock()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;
        animator.SetBool(isOpenParam, true);
    }

    // เผื่อกรณีต้องการล็อกกลับ (เช่นรีเซ็ตด่าน)
    public void Lock()
    {
        IsUnlocked = false;
        animator.SetBool(isOpenParam, false);
    }
}
