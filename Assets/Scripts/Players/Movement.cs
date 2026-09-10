using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2f;
    public float acceleration = 10f;

    [Header("Crouching")]
    public Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    public Vector3 standScale = new Vector3(1, 1f, 1);
    public float crouchHeightOffset = 0.5f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public bool IsCrouching { get; private set; }

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;   // ลดต่อวินาทีตอนวิ่ง
    public float staminaRegenRate = 15f;   // เพิ่มต่อวินาทีตอนไม่วิ่ง
    private float currentStamina;
    private bool isExhausted = false;      // true ถ้าหมดแล้วต้องรอเต็มถังก่อนวิ่งใหม่ได้
    private bool wasRunningLastFrame = false;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleCrouchInput();

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = Vector3.ClampMagnitude(direction, 1f);

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift)
                          && direction.magnitude > 0.1f
                          && !IsCrouching;

        // วิ่งได้ก็ต่อเมื่อ: กด shift + ขยับ + ไม่ย่อ + ยังไม่หมดสภาพ + มี stamina เหลือ
        bool isRunning = wantsToRun && !isExhausted && currentStamina > 0f;

        HandleStamina(isRunning);

        float targetSpeed;
        if (IsCrouching)
        {
            targetSpeed = crouchSpeed;
        }
        else
        {
            targetSpeed = isRunning ? runSpeed : walkSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        controller.Move(direction * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleStamina(bool isRunning)
    {
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true; // ล็อกไว้ วิ่งไม่ได้จนกว่าจะเต็มถัง
            }
        }
        else
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);

            if (isExhausted && currentStamina >= maxStamina)
            {
                isExhausted = false; // ฟื้นแล้ว วิ่งได้อีกครั้ง
            }
        }

        // ตรวจจับ "จังหวะเปลี่ยนสถานะ" วิ่ง <-> ไม่วิ่ง เพื่อสั่ง UI แค่ตอนเปลี่ยนเท่านั้น
        if (isRunning && !wasRunningLastFrame)
        {
            StaminaUI.Instance?.ShowImmediate();
        }
        else if (!isRunning && wasRunningLastFrame)
        {
            StaminaUI.Instance?.HideDelayed();
        }
        wasRunningLastFrame = isRunning;

        StaminaUI.Instance?.UpdateStamina(currentStamina / maxStamina);
    }

    void HandleCrouchInput()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - crouchHeightOffset, transform.position.z);
            IsCrouching = true;
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = standScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + crouchHeightOffset, transform.position.z);
            IsCrouching = false;
        }
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    public void SetFrozen(bool frozen)
    {
        this.enabled = !frozen;
    }
}
