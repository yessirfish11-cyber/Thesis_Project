using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;

public class QTEController : MonoBehaviour
{
    private static QTEController _instance;
    public static QTEController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<QTEController>();
            return _instance;
        }
    }

    [Header("UI Elements")]
    public GameObject qtePanel;
    public Slider progressSlider;

    [Header("UI อื่นที่ต้องซ่อนระหว่าง QTE")]
    public List<GameObject> uiPanelsToHide;

    [Header("ตั้งค่า QTE")]
    public float timeLimit = 4f;
    public float progressPerPress = 0.12f;
    public float decayPerSecond = 0.15f;
    public KeyCode qteKey = KeyCode.Space;

    private bool isRunning = false;
    private float timer;
    private float progress;
    private Action<bool> onComplete;
    public float qteStandoffDistance = 1.5f;

    private GameObject currentKiller;
    private KillerController killerController;
    private KillerAttack killerAttack;

    private GameObject currentPlayer;
    private PlayerMovement playerMovement;
    private MouseLook playerMouseLook;

    private List<bool> uiPanelsPreviousState = new List<bool>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (qtePanel != null) qtePanel.SetActive(false);
    }

    // เพิ่ม parameter player เข้ามา
    public void StartQTE(Action<bool> callback, GameObject killer, GameObject player)
    {
        if (isRunning) return;

        onComplete = callback;
        timer = timeLimit;
        progress = 0f;
        isRunning = true;

        if (qtePanel != null) qtePanel.SetActive(true);
        if (progressSlider != null) progressSlider.value = 0f;

        HideOtherUI();

        currentKiller = killer;
        if (currentKiller != null)
        {
            killerController = currentKiller.GetComponent<KillerController>();
            killerAttack = currentKiller.GetComponent<KillerAttack>();

            // หยุด NavMeshAgent ให้สนิทจริงๆ ก่อน (เคลียร์ velocity ค้าง)
            NavMeshAgent agent = currentKiller.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero; // เคลียร์แรงเฉื่อยทันที

                // ดันตำแหน่งให้ถอยห่างจาก Player ในระยะที่กำหนด
                if (player != null)
                {
                    Vector3 dirAwayFromPlayer = (currentKiller.transform.position - player.transform.position).normalized;
                    Vector3 targetPos = player.transform.position + dirAwayFromPlayer * qteStandoffDistance;

                    agent.Warp(targetPos); // ใช้ Warp แทน transform.position ตรงๆ เพื่อไม่ให้ NavMesh งง
                    currentKiller.transform.LookAt(new Vector3(player.transform.position.x, currentKiller.transform.position.y, player.transform.position.z)); // หันหน้าเข้าหา Player
                }
            }

            if (killerController != null) killerController.SetFrozen(true);
            if (killerAttack != null) killerAttack.SetFrozen(true);
        }

        currentPlayer = player;
        if (currentPlayer != null)
        {
            playerMovement = currentPlayer.GetComponent<PlayerMovement>();
            playerMouseLook = currentPlayer.GetComponentInChildren<MouseLook>();

            if (playerMovement != null) playerMovement.SetFrozen(true);
            if (playerMouseLook != null) playerMouseLook.SetFrozen(true);
        }
    }

    void HideOtherUI()
    {
        uiPanelsPreviousState.Clear();
        foreach (var panel in uiPanelsToHide)
        {
            if (panel != null)
            {
                uiPanelsPreviousState.Add(panel.activeSelf);
                panel.SetActive(false);
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

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetKeyDown(qteKey))
        {
            progress += progressPerPress;
        }

        progress -= decayPerSecond * Time.deltaTime;
        progress = Mathf.Clamp01(progress);

        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }

        if (progress >= 1f)
        {
            Complete(true);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Complete(false);
        }
    }

    void Complete(bool success)
    {
        isRunning = false;
        if (qtePanel != null) qtePanel.SetActive(false);

        RestoreOtherUI();

        // ปลด Killer เสมอ ไม่ว่าจะรอดหรือตาย
        if (killerController != null) killerController.SetFrozen(false);
        if (killerAttack != null) killerAttack.SetFrozen(false);

        // ปลด Player เสมอเช่นกัน
        // ถ้าตาย ตัวนี้จะถูก SetActive(false) โดย PlayerSwitcher อยู่แล้ว ไม่มีผลกระทบ
        if (playerMovement != null) playerMovement.SetFrozen(false);
        if (playerMouseLook != null) playerMouseLook.SetFrozen(false);

        currentKiller = null;
        killerController = null;
        killerAttack = null;
        currentPlayer = null;
        playerMovement = null;
        playerMouseLook = null;

        onComplete?.Invoke(success);
        onComplete = null;
    }
}
