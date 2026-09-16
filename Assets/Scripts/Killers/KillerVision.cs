using UnityEngine;

public class KillerVision : MonoBehaviour
{
    [Header("การมองเห็น (Detection)")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public Transform eyePoint;

    public Transform DetectedPlayer { get; private set; }
    public bool CanSeePlayer { get; private set; }

    // เพิ่ม property นี้ ให้ KillerController มาสั่งได้ว่าตอนนี้ควรสะสมค่าดวงตาไหม
    public bool ShouldTrackDetection { get; set; } = true;

    void Start()
    {
        if (eyePoint == null)
            eyePoint = transform;
    }

    void Update()
    {
        CheckForPlayer();
    }

    void CheckForPlayer()
    {
        CanSeePlayer = false;

        Collider[] targetsInRadius = Physics.OverlapSphere(eyePoint.position, viewRadius, playerMask);

        foreach (Collider target in targetsInRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - eyePoint.position).normalized;

            if (Vector3.Angle(eyePoint.forward, dirToTarget) < viewAngle / 2f)
            {
                float distToTarget = Vector3.Distance(eyePoint.position, targetTransform.position);

                if (!Physics.Raycast(eyePoint.position, dirToTarget, distToTarget, obstacleMask))
                {
                    CanSeePlayer = true;
                    DetectedPlayer = targetTransform;

                    // เรียกเสมอ ไม่ว่า Killer จะ Patrol หรือ Chase - ไม่ต้องเช็ค ShouldTrackDetection แล้ว
                    PlayerDetection detection = targetTransform.GetComponent<PlayerDetection>();
                    if (detection != null)
                    {
                        detection.ReportSeen();
                    }

                    return;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Transform origin = eyePoint != null ? eyePoint : transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f, origin);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f, origin);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position, origin.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(origin.position, origin.position + rightBoundary * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees, Transform origin)
    {
        angleInDegrees += origin.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
