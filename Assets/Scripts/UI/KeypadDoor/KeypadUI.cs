using UnityEngine;
using TMPro;
using System.Text;

public class KeypadUI : MonoBehaviour
{
    [Header("Correct Code")]
    public string correctCode = "1234"; // ตั้งรหัส 4 หลักตามต้องการ

    [Header("UI Elements")]
    public GameObject panel;          // Panel ทั้งหน้าต่าง (เปิด/ปิด)
    public TMP_Text displayText;          // ช่องแสดงตัวเลขที่กรอก (ใช้ TMP_Text แทนได้ถ้าใช้ TextMeshPro)
    public TMP_Text feedbackText;         // ข้อความ "ถูกต้อง" / "Error"

    [Header("System")]
    public int codeLength = 4;

    private StringBuilder enteredCode = new StringBuilder();
    private KeypadDoor currentDoor;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    public void Open(KeypadDoor door)
    {
        currentDoor = door;
        enteredCode.Clear();
        UpdateDisplay();

        if (feedbackText != null) feedbackText.text = "";
        if (panel != null) panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
    }

    // เรียกจากปุ่มตัวเลข 0-9 (ตั้งใน OnClick ของปุ่มแต่ละอันใน Inspector)
    public void PressNumber(string number)
    {
        if (enteredCode.Length >= codeLength) return;

        enteredCode.Append(number);
        UpdateDisplay();

        if (enteredCode.Length >= codeLength)
        {
            CheckCode();
        }
    }

    // เรียกจากปุ่ม Clear/ลบ
    public void ClearCode()
    {
        enteredCode.Clear();
        UpdateDisplay();
        if (feedbackText != null) feedbackText.text = "";
    }

    // เรียกจากปุ่ม Backspace (ลบตัวสุดท้าย)
    public void Backspace()
    {
        if (enteredCode.Length > 0)
        {
            enteredCode.Length -= 1;
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (displayText != null)
        {
            displayText.text = enteredCode.ToString();
        }
    }

    void CheckCode()
    {
        if (enteredCode.ToString() == correctCode)
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Correct!!";
                feedbackText.color = Color.green;
            }

            if (currentDoor != null)
            {
                currentDoor.Unlock();
            }

            Invoke(nameof(Close), 1f); // รอ 1 วิ ให้เห็นข้อความก่อนปิดหน้าต่าง
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Error!!";
                feedbackText.color = Color.red;
            }

            Invoke(nameof(ClearCode), 0.8f); // รอสักครู่แล้วเคลียร์ให้กรอกใหม่
        }
    }
}
