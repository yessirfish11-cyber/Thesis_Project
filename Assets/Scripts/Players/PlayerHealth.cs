using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("ค่าพลังชีวิต")]
    public int maxLives = 2;

    [Header("QTE Settings")]
    [Range(0f, 1f)]
    public float qteChanceAfterFirstSave = 0.5f; // โอกาสได้ QTE ต่อ (แทนตายทันที) หลังเคยรอด QTE มาแล้ว

    public int CurrentLives { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;

    private bool hasUsedFirstQTE = false; // เคยเจอ QTE ตอนเหลือ 1 ชีวิตแล้วหรือยัง
    private GameObject attackingKiller;

    void Awake()
    {
        CurrentLives = maxLives;
    }

    void OnEnable()
    {
        if (HealthBarUI.Instance != null)
        {
            HealthBarUI.Instance.SetTarget(this);
        }
        UpdateLowHealthEffect();
    }

    void OnDisable()
    {
        // ปิด Effect ทันทีตอนตัวนี้ถูกปิด (สลับตัวละคร/ตาย) กันเอฟเฟกต์ค้าง
        if (LowHealthEffect.Instance != null)
        {
            LowHealthEffect.Instance.StopPulse();
        }
    }

    public void ResetHealth()
    {
        CurrentLives = maxLives;
        IsDead = false;
        hasUsedFirstQTE = false; // รีเซ็ตตอนสลับตัวละครใหม่ด้วย
        OnHealthChanged?.Invoke(CurrentLives, maxLives);
        UpdateLowHealthEffect();
    }

    // เรียกจาก KillerAttack ทุกครั้งที่ตีโดนผู้เล่น
    public void TakeHit(GameObject killer)
    {
        if (IsDead) return;

        attackingKiller = killer;

        if (CurrentLives >= maxLives)
        {
            CurrentLives = 1;
            OnHealthChanged?.Invoke(CurrentLives, maxLives);
            UpdateLowHealthEffect();
            return;
        }

        if (!hasUsedFirstQTE)
        {
            hasUsedFirstQTE = true;
            RequestQTE();
        }
        else
        {
            float roll = UnityEngine.Random.value;
            if (roll <= qteChanceAfterFirstSave)
            {
                RequestQTE();
            }
            else
            {
                Die();
            }
        }
    }

    void RequestQTE()
    {
        if (QTEController.Instance != null)
        {
            QTEController.Instance.StartQTE(OnQTEResult, attackingKiller, gameObject); // เพิ่ม gameObject (ตัว Player)
        }
        else
        {
            Debug.LogWarning("ไม่พบ QTEController ในฉาก! ผู้เล่นจะตายทันที");
            Die();
        }
    }

    void OnQTEResult(bool success)
    {
        if (success)
        {
            // รอดจาก QTE ยังเหลือ 1 ชีวิตเท่าเดิม
            OnHealthChanged?.Invoke(CurrentLives, maxLives);
            UpdateLowHealthEffect();
        }
        else
        {
            Die();
        }
    }

    void UpdateLowHealthEffect()
    {
        if (LowHealthEffect.Instance == null) return;

        if (CurrentLives == 1 && !IsDead)
        {
            LowHealthEffect.Instance.StartPulse();
        }
        else
        {
            LowHealthEffect.Instance.StopPulse();
        }
    }

    void Die()
    {
        IsDead = true;

        if (LowHealthEffect.Instance != null)
        {
            LowHealthEffect.Instance.StopPulse();
        }

        OnPlayerDeath?.Invoke();
    }
}
