using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterControllerWithNavMesh : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Animation Settings")]
    public Animator animator;

    private NavMeshAgent navMeshAgent;
    private Vector3 targetPosition;
    public bool isRunning;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = walkSpeed;
    }

    void Update()
    {
        UpdateAnimator();

        // Optional: Check if character reached the destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                // Reached destination
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    public void SetTargetPosition(Vector3 position, bool running = false)
    {
        targetPosition = position;
        isRunning = running;

        navMeshAgent.speed = isRunning ? runSpeed : walkSpeed;
        navMeshAgent.SetDestination(targetPosition);
    }

    private void UpdateAnimator()
    {
        float speed = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
        animator.SetFloat("Speed", speed);

        animator.SetBool("IsRunning", isRunning);
    }
}
