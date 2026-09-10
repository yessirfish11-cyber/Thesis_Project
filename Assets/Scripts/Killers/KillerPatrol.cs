using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KillerPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 3f;

    private NavMeshAgent agent;
    private KillerAttack attack;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    public bool IsWaitingAtPoint => isWaiting;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<KillerAttack>(); // cache ไว้ครั้งเดียว

    }

    // เรียกจาก KillerController ตอนเข้าสถานะ Patrol
    public void BeginPatrol()
    {
        if (!agent.isOnNavMesh) return;

        agent.speed = patrolSpeed;
        isWaiting = false;
        waitTimer = 0f;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    // เรียกทุกเฟรมจาก KillerController ตอนอยู่สถานะ Patrol
    public void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;
        if (!agent.isOnNavMesh) return;
        if (attack != null && attack.IsBusy) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
    }

    void GoToNextPatrolPoint()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    public void CancelWaiting()
    {
        isWaiting = false;
        waitTimer = 0f;
    }
}
