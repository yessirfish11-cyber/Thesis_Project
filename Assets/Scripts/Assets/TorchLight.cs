using UnityEngine;
using UnityEngine.InputSystem;

public class TorchLight : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleAction;
    [SerializeField] private Light torchLight;

    [Header("Battery Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float drainRatePerSecond = 5f;
    [SerializeField] private float currentEnergy;

    private PlayerInventory inventory;
    private bool wasEnabledLastFrame = false;

    private void Awake()
    {
        inventory = PlayerInventory.Instance;
        currentEnergy = maxEnergy;
        torchLight.enabled = false;
    }

    private void OnEnable()
    {
        toggleAction.action.Enable();
        toggleAction.action.performed += OnToggle;
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= OnToggle;
        toggleAction.action.Disable();
    }

    private void Update()
    {
        bool hasTorch = inventory != null && inventory.HasItemOfType(ItemPickup.ItemType.Torch);

        if (!hasTorch)
        {
            TorchBatteryUI.Instance?.HideImmediate();
            wasEnabledLastFrame = false;
        }
        else
        {
            if (torchLight.enabled && !wasEnabledLastFrame)
            {
                TorchBatteryUI.Instance?.ShowImmediate();
            }
            else if (!torchLight.enabled && wasEnabledLastFrame)
            {
                TorchBatteryUI.Instance?.HideDelayed();
            }
            wasEnabledLastFrame = torchLight.enabled;
        }

        if (torchLight.enabled)
        {
            currentEnergy -= drainRatePerSecond * Time.deltaTime;
            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                torchLight.enabled = false;
            }
        }
        // ลบส่วน regen ออกแล้ว - พลังงานจะไม่ฟื้นเองตอนไฟปิด

        TorchBatteryUI.Instance?.UpdateBattery(GetEnergyPercent());
    }

    private void OnToggle(InputAction.CallbackContext ctx)
    {
        if (inventory == null || !inventory.HasItemOfType(ItemPickup.ItemType.Torch))
            return;

        if (!torchLight.enabled && currentEnergy <= 0f)
            return;

        torchLight.enabled = !torchLight.enabled;
    }

    public void Recharge(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
    }

    public float GetEnergyPercent() => currentEnergy / maxEnergy;
}
