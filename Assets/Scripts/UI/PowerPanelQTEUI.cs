using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class PowerPanelQTEUI : MonoBehaviour
{
    private static PowerPanelQTEUI _instance;
    public static PowerPanelQTEUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<PowerPanelQTEUI>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject panel;             // Panel หลักของมินิเกม (อยู่ตลอดเวลาที่เล่น)
    public GameObject trackBarGroup;      // กลุ่ม TrackBar+Marker+TargetZone (ตัวที่จะโผล่/หาย)
    public RectTransform trackBar;
    public RectTransform marker;
    public RectTransform targetZone;
    public Slider progressSlider;

    [Header("UI อื่นที่ต้องซ่อน (ยกเว้นหลอดดวงตา)")]
    public List<GameObject> uiPanelsToHide;

    [Header("ตั้งค่าตัวชี้")]
    public float markerSpeed = 1.2f;

    [Header("ตั้งค่าโซนเป้าหมาย")]
    [Range(0.05f, 0.4f)]
    public float zoneWidth = 0.15f;

    [Header("ตั้งค่า Progress")]
    public float progressPerSecond = 0.08f;
    public float penaltyPerMiss = 0.1f;

    [Header("ตั้งค่าจังหวะหาย-โผล่")]
    public float hideDuration = 1f; // ระยะเวลาที่ TrackBar หายไปก่อนโผล่กลับมาใหม่

    [Header("ตั้งค่าปุ่มออก")]
    public KeyCode exitKey = KeyCode.E;

    private bool isRunning = false;
    private bool trackBarVisible = true;
    private float progress = 0f;
    private float zoneCenter = 0.5f;
    private float markerStartTime = 0f; // ใช้รีเซ็ตจังหวะ Marker ทุกครั้งที่โผล่ใหม่

    private GameObject currentPlayer;
    private PlayerMovement playerMovement;
    private MouseLook playerMouseLook;
    private Action<bool> onComplete;
    private Coroutine hideRoutine;

    private List<bool> uiPanelsPreviousState = new List<bool>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (panel != null) panel.SetActive(false);
    }

    public void Open(Action<bool> callback, GameObject player)
    {
        onComplete = callback;
        progress = 0f;
        isRunning = true;
        currentPlayer = player;

        if (panel != null) panel.SetActive(true);
        if (progressSlider != null) progressSlider.value = 0f;

        HideOtherUI();
        FreezePlayer(player, true);
        ShowTrackBar();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetKeyDown(exitKey))
        {
            Complete(false);
            return;
        }

        // Progress ไหลเพิ่มเองตลอดเวลา ไม่ว่า TrackBar จะโผล่อยู่หรือหายไปก็ตาม
        progress += progressPerSecond * Time.deltaTime;
        progress = Mathf.Clamp01(progress);
        if (progressSlider != null) progressSlider.value = progress;

        if (progress >= 1f)
        {
            Complete(true);
            return;
        }

        // อัปเดต Marker/คลิก เฉพาะตอน TrackBar โผล่อยู่เท่านั้น
        if (trackBarVisible)
        {
            float markerPos = Mathf.PingPong((Time.time - markerStartTime) * markerSpeed, 1f);
            UpdateMarkerVisual(markerPos);
            UpdateZoneVisual();

            if (Input.GetMouseButtonDown(0))
            {
                bool isInZone = Mathf.Abs(markerPos - zoneCenter) <= zoneWidth / 2f;

                if (isInZone)
                {
                    HideTrackBarTemporarily(); // สำเร็จ -> หายไปชั่วคราว
                }
                else
                {
                    progress -= penaltyPerMiss; // พลาด -> โดนหัก TrackBar ยังอยู่
                    progress = Mathf.Clamp01(progress);
                    if (progressSlider != null) progressSlider.value = progress;
                }
            }
        }
    }

    void HideTrackBarTemporarily()
    {
        trackBarVisible = false;
        if (trackBarGroup != null) trackBarGroup.SetActive(false);

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(ShowTrackBarAfterDelay());
    }

    IEnumerator ShowTrackBarAfterDelay()
    {
        yield return new WaitForSeconds(hideDuration);
        ShowTrackBar();
    }

    void ShowTrackBar()
    {
        trackBarVisible = true;
        if (trackBarGroup != null) trackBarGroup.SetActive(true);

        RandomizeZone();
        markerStartTime = Time.time; // รีเซ็ตจังหวะ Marker ให้เริ่มนับใหม่จากตำแหน่งเริ่มต้น
    }

    void RandomizeZone()
    {
        float halfZone = zoneWidth / 2f;
        zoneCenter = UnityEngine.Random.Range(halfZone, 1f - halfZone);
    }

    void UpdateMarkerVisual(float normalizedPos)
    {
        if (marker == null || trackBar == null) return;

        float barWidth = trackBar.rect.width;
        float xPos = (normalizedPos - 0.5f) * barWidth;
        marker.anchoredPosition = new Vector2(xPos, marker.anchoredPosition.y);
    }

    void UpdateZoneVisual()
    {
        if (targetZone == null || trackBar == null) return;

        float barWidth = trackBar.rect.width;
        float xPos = (zoneCenter - 0.5f) * barWidth;
        targetZone.anchoredPosition = new Vector2(xPos, targetZone.anchoredPosition.y);
        targetZone.sizeDelta = new Vector2(barWidth * zoneWidth, targetZone.sizeDelta.y);
    }

    void HideOtherUI()
    {
        uiPanelsPreviousState.Clear();
        foreach (var p in uiPanelsToHide)
        {
            if (p != null)
            {
                uiPanelsPreviousState.Add(p.activeSelf);
                p.SetActive(false);
            }
        }
    }

    void RestoreOtherUI()
    {
        for (int i = 0; i < uiPanelsToHide.Count; i++)
        {
            if (uiPanelsToHide[i] != null && i < uiPanelsPreviousState.Count)
            {
                uiPanelsToHide[i].SetActive(uiPanelsPreviousState[i]);
            }
        }
    }

    void FreezePlayer(GameObject player, bool frozen)
    {
        if (player == null) return;

        if (frozen)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerMouseLook = player.GetComponentInChildren<MouseLook>();
        }

        if (playerMovement != null) playerMovement.enabled = !frozen;
        if (playerMouseLook != null) playerMouseLook.enabled = !frozen;
    }

    void Complete(bool success)
    {
        isRunning = false;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (panel != null) panel.SetActive(false);

        RestoreOtherUI();
        FreezePlayer(currentPlayer, false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentPlayer = null;
        playerMovement = null;
        playerMouseLook = null;

        onComplete?.Invoke(success);
        onComplete = null;
    }
}
