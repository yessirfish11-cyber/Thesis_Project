using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(KillerVision))]
[RequireComponent(typeof(KillerPatrol))]
public class KillerController : MonoBehaviour
{
    private enum KillerState
    {
        Patrol,
        Chase
    }

    [Header("Hunt (Chase)")]
    public float chaseSpeed = 5f;
    public float loseSightTime = 3f;

    private NavMeshAgent agent;
    private KillerVision vision;
    private KillerPatrol patrol;

    private KillerState currentState;
    private float loseSightTimer = 0f;
    private KillerAttack attack;

    void Start()   // ให้ Start เหลือแค่สั่งเริ่ม state
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<KillerVision>();
        patrol = GetComponent<KillerPatrol>();
        attack = GetComponent<KillerAttack>(); // เพิ่มบรรทัดนี้

        EnterPatrolState();
    }

    void Update()
    {
        switch (currentState)
        {
            case KillerState.Patrol:
                patrol.UpdatePatrol();

                if (vision.CanSeePlayer)
                {
                    EnterChaseState();
                }
                break;

            case KillerState.Chase:
                HandleChase();
                break;
        }
    }

    void HandleChase()
    {
        // ถ้ากำลังตี/พัก อยู่ ไม่ต้องสั่งเดินทับ ปล่อยให้ KillerAttack คุมเองก่อน
        if (attack != null && attack.IsBusy) return;

        if (vision.CanSeePlayer)
        {
            agent.SetDestination(vision.DetectedPlayer.position);
            loseSightTimer = 0f;
        }
        else
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= loseSightTime)
            {
                EnterPatrolState();
            }
        }
    }

    void EnterPatrolState()
    {
        currentState = KillerState.Patrol;
        patrol.BeginPatrol();
    }

    void EnterChaseState()
    {
        currentState = KillerState.Chase;
        agent.speed = chaseSpeed;

        if (patrol != null)
        {
            patrol.CancelWaiting(); // เพิ่มบรรทัดนี้ - ยกเลิกการรอที่จุด Patrol ทันที
        }
    }

    public void SetFrozen(bool frozen)
    {
        if (agent != null)
        {
            agent.isStopped = frozen;
        }
        this.enabled = !frozen; // หยุด Update loop ทั้งหมดของสมองกลาง (state จะค้างไว้ ไม่รีเซ็ต)
    }
}
