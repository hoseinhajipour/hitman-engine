using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SuspicionMeter : MonoBehaviour
{
    [Header("Suspicion Settings")]
    [SerializeField] private float suspicionIncreaseRate = 10f; // Rate of suspicion increase per second
    [SerializeField] private float suspicionDecreaseRate = 5f; // Rate of suspicion decrease per second
    [SerializeField] private float maxSuspicion = 100f; // Maximum suspicion value
    [SerializeField] private float fovRange = 45f; // Enemy's field of view angle (in degrees)
    [SerializeField] private float detectionRadius = 10f; // Maximum detection radius
    [SerializeField] private Image suspicionBar; // UI element to show suspicion level

    private float currentSuspicion = 0f; // Current suspicion level
    private Transform detectedPlayer; // Detected player's transform
    private bool isFollowingPlayer = false; // Is the enemy following the player?
    public Transform headBone; // Reference to the enemy's head bone

    private void Update()
    {
        // Detect the player using angle and distance
        Transform playerTransform = CanSeePlayer();

        if (playerTransform != null)
        {
            detectedPlayer = playerTransform;
            IncreaseSuspicion();
        }
        else
        {
            detectedPlayer = null;
            DecreaseSuspicion();
        }

        // Update the UI
        UpdateSuspicionUI();

        // Trigger follow behavior when suspicion reaches maximum
        if (currentSuspicion >= maxSuspicion && !isFollowingPlayer)
        {
            FollowPlayer();
        }
    }

    private Transform CanSeePlayer()
    {
        if (headBone == null)
        {
            Debug.LogError("Head bone is not assigned!");
            return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
            return null;
        }

        Vector3 origin = headBone.position; // نقطه شروع مخروط
        Vector3 direction = headBone.forward; // جهت مخروط
        float maxDistance = detectionRadius; // حداکثر فاصله دید
        float maxRadius = detectionRadius / 2; // شعاع مخروط
        LayerMask obstacleLayer = LayerMask.GetMask("Default"); // تنظیم لایه موانع

        // استفاده از نسخه جدید ConeCastAll
        RaycastHit[] hits = ConeCastExtension.ConeCastAll(origin, maxRadius, direction, maxDistance, fovRange, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player detected within cone and no obstacles in the way!");
                return hit.collider.transform;
            }
        }

        return null;
    }




    private void IncreaseSuspicion()
    {
        currentSuspicion += suspicionIncreaseRate * Time.deltaTime;
        currentSuspicion = Mathf.Clamp(currentSuspicion, 0, maxSuspicion);
    }

    private void DecreaseSuspicion()
    {
        currentSuspicion -= suspicionDecreaseRate * Time.deltaTime;
        currentSuspicion = Mathf.Clamp(currentSuspicion, 0, maxSuspicion);
    }

    private void UpdateSuspicionUI()
    {
        if (suspicionBar != null)
        {
            suspicionBar.fillAmount = currentSuspicion / maxSuspicion;
        }
    }

    private void FollowPlayer()
    {
        isFollowingPlayer = true;
        Debug.Log("Enemy is now following the player!");
        if (detectedPlayer != null)
        {
            GameObject Player = GameObject.FindGameObjectWithTag("Player");
            transform.gameObject.GetComponent<NavMeshAgent>().SetDestination(Player.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (headBone == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headBone.position, detectionRadius);

        Gizmos.color = Color.red;
        Vector3 forward = headBone.forward * detectionRadius;

        // Draw the field of view cone
        Vector3 leftBoundary = Quaternion.Euler(0, -fovRange / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fovRange / 2, 0) * forward;

        Gizmos.DrawRay(headBone.position, leftBoundary);
        Gizmos.DrawRay(headBone.position, rightBoundary);

        // Draw a wire arc to represent the field of view
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = -fovRange / 2 + (fovRange / segments) * i;
            float angle2 = -fovRange / 2 + (fovRange / segments) * (i + 1);

            Vector3 point1 = Quaternion.Euler(0, angle1, 0) * forward + headBone.position;
            Vector3 point2 = Quaternion.Euler(0, angle2, 0) * forward + headBone.position;

            Gizmos.DrawLine(point1, point2);
        }
    }
}
