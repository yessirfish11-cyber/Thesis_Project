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

    [Header("การไล่ล่า (Chase)")]
    public float chaseSpeed = 5f;

    [Header("ป้องกันสลับ State เร็วเกินไป")]
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
        reEngageTimer -= Time.deltaTime;

        switch (currentState)
        {
            case KillerState.Patrol:
                patrol.UpdatePatrol();

                if (vision.CanSeePlayer && reEngageTimer <= 0f)
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

        bool targetStillValid = currentTargetDetection != null
                                 && currentTargetDetection.gameObject.activeInHierarchy;

        if (targetStillValid)
        {
            PlayerHealth targetHealth = currentTargetDetection.GetComponent<PlayerHealth>();
            if (targetHealth != null && targetHealth.IsDead)
            {
                targetStillValid = false;
            }
        }

        if (!targetStillValid)
        {
            EnterPatrolState();
            return;
        }

        if (vision.CanSeePlayer)
        {
            agent.isStopped = false;
            agent.SetDestination(vision.DetectedPlayer.position);
        }
        else
        {
            // ตรงนี้คือส่วนสำคัญ - ต้องมีอยู่
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (currentTargetDetection.IsEmpty)
        {
            EnterPatrolState();
        }
    }

    void EnterChaseState()
    {
        currentState = KillerState.Chase;
        agent.speed = chaseSpeed;
        agent.isStopped = false;

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
        agent.isStopped = false;
        agent.speed = patrol.patrolSpeed;
        patrol.BeginPatrol();
        currentTargetDetection = null;

        reEngageTimer = reEngageCooldown;
    }

    public void SetFrozen(bool frozen)
    {
        if (agent != null)
        {
            agent.isStopped = frozen;
        }
        this.enabled = !frozen;
    }
}
