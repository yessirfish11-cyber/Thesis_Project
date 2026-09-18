using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(KillerVision))]
public class KillerAttack : MonoBehaviour
{
    [Header("Attack")]
    public float attackRange = 2f;
    public float attackReachBuffer = 0.5f;
    public float attackCooldown = 1.5f;      // เวลารอก่อนจะตีได้อีกครั้ง (นับตั้งแต่ตีครั้งก่อน)
    public float damageDelay = 0.5f;         // ดีเลย์ก่อนดาเมจจะลง (ให้ตรงจังหวะแอนิเมชันยกมือ/ฟัน)
    public float idleAfterAttack = 1f;

    [Header("Animator")]
    public Animator animator;
    public string speedParam = "Speed";
    public string attackTriggerParam = "Attack";

    private KillerVision vision;
    private NavMeshAgent agent;
    private float attackTimer = 999f;
    private KillerPatrol patrol;

    // true ตลอดช่วง "กำลังตี" + "พัก idle หลังตี" -> KillerController จะไม่สั่งเดินทับ
    public bool IsBusy { get; private set; }

    void Awake()
    {
        vision = GetComponent<KillerVision>();
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<KillerPatrol>(); // เพิ่มบรรทัดนี้
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        UpdateAnimatorSpeed();

        if (!IsBusy)
        {
            TryAttack();
        }
    }

    void UpdateAnimatorSpeed()
    {
        if (animator == null || agent == null) return;

        float speed;
        bool isWaitingAtPatrolPoint = patrol != null && patrol.IsWaitingAtPoint;

        // ต้องมี || agent.isStopped ตรงนี้ด้วย
        if (IsBusy || isWaitingAtPatrolPoint || agent.isStopped)
        {
            speed = 0f;
        }
        else
        {
            speed = agent.velocity.magnitude;
        }

        animator.SetFloat(speedParam, speed);
    }

    void TryAttack()
    {
        if (!vision.CanSeePlayer || vision.DetectedPlayer == null) return;

        float distance = Vector3.Distance(transform.position, vision.DetectedPlayer.position);

        if (distance <= attackRange && attackTimer >= attackCooldown)
        {
            StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        IsBusy = true;
        attackTimer = 0f;

        // หยุดเดินสนิท ยืนตีอยู่กับที่
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger(attackTriggerParam);
        }

        // รอให้ตรงจังหวะแอนิเมชันก่อนค่อยลงดาเมจจริง
        yield return new WaitForSeconds(damageDelay);

        if (vision.DetectedPlayer != null)
        {
            float currentDistance = Vector3.Distance(transform.position, vision.DetectedPlayer.position);

            if (currentDistance <= attackRange)
            {
                PlayerHealth targetHealth = vision.DetectedPlayer.GetComponent<PlayerHealth>();
                if (targetHealth != null && !targetHealth.IsDead)
                {
                    targetHealth.TakeHit(gameObject);
                }
            }
            // ถ้าไกลเกินไปแล้ว -> ไม่ลงดาเมจ (ตีพลาด เพราะผู้เล่นหนีทัน)
        }

        // พัก Idle หลังตีเสร็จ ก่อนจะกลับไปไล่ล่าต่อ (เหมือนคูลดาวน์)
        yield return new WaitForSeconds(idleAfterAttack);

        if (agent != null)
        {
            agent.isStopped = false;
        }
        IsBusy = false;
    }

    public void SetFrozen(bool frozen)
    {
        this.enabled = !frozen;
        if (frozen && agent != null)
        {
            agent.isStopped = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        // วงกลมสีแดง = ระยะที่เริ่มโจมตีได้ (attackRange)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // วงกลมสีเหลือง = ระยะเผื่อตอนลงดาเมจจริง (ถ้าใส่ attackReachBuffer ไว้)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange + attackReachBuffer);
    }
}
