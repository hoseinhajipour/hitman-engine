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
    public ActionManager PatrolActions; // Actions to perform when the player is not seen
    public ActionManager SightActions; // Actions to perform when the player is seen
    public ActionManager AttackActions; // Actions to perform when the player is near

    [Header("Guard Mode")]
    public bool isGuardMode = false; // Guard mode state

    private void Start()
    {
        if (headBone == null)
        {
            headBone = transform.Find("Head");
            if (headBone == null)
            {
                Debug.LogError("Head bone not found! Assign it manually in the Inspector.");
            }
        }

        // Start patrolling by default
        PatrolActions.RunActions(gameObject);
    }

    private void Update()
    {
        if (isGuardMode)
        {
            // Stop PatrolActions when entering Guard Mode
            if (PatrolActions.IsRunning())
            {
                PatrolActions.StopActions();
                SightActions.RunActions(gameObject);
            }

            // Guard Mode: Detect and react to the player
            if (IsPlayerInSight())
            {
                if (IsPlayerInAttackRange())
                {
                    if (!AttackActions.IsRunning())
                    {
                        AttackActions.RunActions(gameObject);
                    }
                }
                else
                {
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
                // Player not in sight, stop attacking and sight actions
                if (SightActions.IsRunning())
                {
                    SightActions.StopActions();
                }
                if (AttackActions.IsRunning())
                {
                    AttackActions.StopActions();
                }
            }
        }
        else
        {
            // Default Mode: Only patrol, ignore the player
            if (!PatrolActions.IsRunning())
            {
                PatrolActions.RunActions(gameObject);
            }

            // Stop any attack or sight actions if they were running
            if (SightActions.IsRunning())
            {
                SightActions.StopActions();
            }
            if (AttackActions.IsRunning())
            {
                AttackActions.StopActions();
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

    // Public method to toggle Guard Mode
    public void SetGuardMode(bool guardMode)
    {
        isGuardMode = guardMode;

        if (isGuardMode)
        {
            // Enter Guard Mode: Stop patrolling and start detecting the player
            PatrolActions.StopActions();

            if (IsPlayerInSight())
            {
                SightActions.RunActions(gameObject);
            }
        }
        else
        {
            // Exit Guard Mode: Stop attacking and sight actions, return to patrolling
            SightActions.StopActions();
            AttackActions.StopActions();

            if (!PatrolActions.IsRunning())
            {
                PatrolActions.RunActions(gameObject);
            }
        }
    }
}