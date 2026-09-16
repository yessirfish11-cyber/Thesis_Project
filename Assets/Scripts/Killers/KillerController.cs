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
    public float reEngageCooldown = 1f;

    private NavMeshAgent agent;
    private KillerVision vision;
    private KillerPatrol patrol;
    private KillerAttack attack;

    private KillerState currentState;
    private PlayerDetection currentTargetDetection;
    private float reEngageTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<KillerVision>();
        patrol = GetComponent<KillerPatrol>();
        attack = GetComponent<KillerAttack>();

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
                    PlayerDetection detection = vision.DetectedPlayer.GetComponent<PlayerDetection>();
                    if (detection != null && detection.IsFullyDetected)
                    {
                        EnterChaseState();
                    }
                }
                break;

            case KillerState.Chase:
                HandleChase();
                break;
        }
    }

    void HandleChase()
    {
        if (attack != null && attack.IsBusy) return;

        if (vision.CanSeePlayer)
        {
            agent.SetDestination(vision.DetectedPlayer.position);
        }

        if (currentTargetDetection != null && currentTargetDetection.IsEmpty)
        {
            EnterPatrolState();
        }
    }

    void EnterChaseState()
    {
        currentState = KillerState.Chase;
        agent.speed = chaseSpeed;

        if (patrol != null)
        {
            patrol.CancelWaiting();
        }

        if (vision.DetectedPlayer != null)
        {
            currentTargetDetection = vision.DetectedPlayer.GetComponent<PlayerDetection>();
        }
    }

    void EnterPatrolState()
    {
        currentState = KillerState.Patrol;
        agent.speed = patrol.patrolSpeed; // ตั้งความเร็วตรงนี้ทันที ไม่รอ BeginPatrol()
        patrol.BeginPatrol();
        currentTargetDetection = null;

        reEngageTimer = reEngageCooldown; // เริ่มนับเวลาพัก ก่อนจะเข้า Chase ใหม่ได้
    }

    // ฟังก์ชันนี้หายไปตอนแก้รอบก่อน - เพิ่มกลับเข้ามา (QTEController เรียกใช้ตอนหยุด Killer ระหว่าง QTE)
    public void SetFrozen(bool frozen)
    {
        if (agent != null)
        {
            agent.isStopped = frozen;
        }
        this.enabled = !frozen;
    }
}
