using UnityEngine;

public class Perception : MonoBehaviour
{
    [Header("Vision Settings")]
    public Transform headBone; // Reference to the head bone of the NPC
    public float detectionRange = 15f; // Range of vision
    [Range(0, 360)] public float detectionAngle = 180f; // Field of view angle
    public LayerMask playerLayer; // Layer for the player
    public float attackRange = 2f; // Range within which to trigger an attack

    [Header("Actions")]
    public ActionManager SightActions; // Actions to perform when the player is seen
    public ActionManager PatrolActions; // Actions to perform when the player is not seen
    public ActionManager AttackActions; // Actions to perform when the player is near

    private void Start()
    {
        // Automatically find the head bone if not assigned
        if (headBone == null)
        {
            headBone = transform.Find("Head"); // Example for default name
            if (headBone == null)
            {
                Debug.LogError("Head bone not found! Assign it manually in the Inspector.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (headBone != null)
        {
            // Draw a green cone to represent the field of view
            Gizmos.color = Color.green;

            Vector3 forward = headBone.forward * detectionRange;

            Quaternion leftRayRotation = Quaternion.Euler(0, -detectionAngle / 2, 0);
            Quaternion rightRayRotation = Quaternion.Euler(0, detectionAngle / 2, 0);

            Vector3 leftRay = leftRayRotation * forward;
            Vector3 rightRay = rightRayRotation * forward;

            Gizmos.DrawRay(headBone.position, forward);
            Gizmos.DrawRay(headBone.position, leftRay);
            Gizmos.DrawRay(headBone.position, rightRay);

            Gizmos.DrawWireSphere(headBone.position, detectionRange);

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headBone.position, attackRange);
        }
    }

    private void Update()
    {
        if (IsPlayerInSight())
        {
            if (IsPlayerInAttackRange())
            {
                // اگر بازیکن در محدوده حمله است، اکشن حمله را اجرا کن
                if (!AttackActions.IsRunning())
                {
                    AttackActions.RunActions(gameObject);
                }
            }
            else
            {
                // اگر بازیکن در محدوده حمله نیست، اکشن حمله را متوقف کن و اکشن دید را اجرا کن
                if (AttackActions.IsRunning())
                {
                    AttackActions.StopActions();
                }
                if (!SightActions.IsRunning())
                {
                    SightActions.RunActions(gameObject);
                }
            }
        }
        else
        {
            // اگر بازیکن در دید نیست، اکشن‌های مربوط به دید و حمله را متوقف کن و اکشن گشت‌زنی را اجرا کن
            if (SightActions.IsRunning())
            {
                SightActions.StopActions();
            }
            if (AttackActions.IsRunning())
            {
                AttackActions.StopActions();
            }
            if (!PatrolActions.IsRunning())
            {
                PatrolActions.RunActions(gameObject);
            }
        }
    }

    private bool IsPlayerInSight()
    {
        if (headBone == null) return false;

        Collider[] playersInRange = Physics.OverlapSphere(headBone.position, detectionRange, playerLayer);
        foreach (var player in playersInRange)
        {
            Vector3 directionToPlayer = (player.transform.position - headBone.position).normalized;
            float angleToPlayer = Vector3.Angle(headBone.forward, directionToPlayer);

            if (angleToPlayer < detectionAngle / 2)
            {
                if (Physics.Raycast(headBone.position, directionToPlayer, out RaycastHit hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Player detected: " + hit.collider.name);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsPlayerInAttackRange()
    {
        Collider[] playersInRange = Physics.OverlapSphere(headBone.position, attackRange, playerLayer);
        foreach (var player in playersInRange)
        {
            if (player.CompareTag("Player"))
            {
                Debug.Log("Player within attack range: " + player.name);
                return true;
            }
        }
        return false;
    }
}
