using UnityEngine;
using UnityEngine.AI;

[ActionCategory("Character")]
[System.Serializable]
public class NavMeshMoveAction : ActionBase
{
    public GameObject target;
    public Vector3 destination;

    [Tooltip("Optional Transform to override the destination position.")]
    public Transform destinationTrans;

    [Tooltip("Wait for the NavMeshAgent to reach the destination before completing the task.")]
    public bool waitToEnd;

    [Tooltip("The speed at which the NavMeshAgent should move.")]
    public float speed = 3.5f;

    [Tooltip("Enable running animation if the target has an Animator component.")]
    public bool Run;

    private const float completionThreshold = 0.5f; // Distance threshold to consider the target near the destination

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("NavMeshMoveAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshMoveAction: No NavMeshAgent component found on target!");
            onComplete?.Invoke();
            return;
        }

        // Set the agent's speed
        agent.speed = speed;

        // Handle Animator for running animation
        CharacterControllerWithNavMesh CharacterControllerWithNavMesh = target.GetComponent<CharacterControllerWithNavMesh>();
        if (CharacterControllerWithNavMesh != null && Run)
        {
            CharacterControllerWithNavMesh.isRunning = true;
        }

        // Update the destination if destinationTrans is provided
        if (destinationTrans != null)
        {
            destination = destinationTrans.position;
        }

        // Set the destination
        agent.SetDestination(destination);

        // Check if we should wait for the NavMeshAgent to reach the destination
        if (waitToEnd)
        {
            target.GetComponent<MonoBehaviour>()?.StartCoroutine(WaitForCompletion(agent, CharacterControllerWithNavMesh, onComplete));
        }
        else
        {
            // Reset the running animation if applicable
            if (CharacterControllerWithNavMesh != null && Run)
            {
                CharacterControllerWithNavMesh.isRunning = false;
            }
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitForCompletion(NavMeshAgent agent, CharacterControllerWithNavMesh CharacterControllerWithNavMesh, System.Action onComplete)
    {
        // Wait until the agent reaches the destination
        while (agent.pathPending || agent.remainingDistance > completionThreshold)
        {
            yield return null; // Wait for the next frame
        }

        // Reset the running animation if applicable
        if (CharacterControllerWithNavMesh != null && Run)
        {
            CharacterControllerWithNavMesh.isRunning = false;
        }

        // Task completed
        onComplete?.Invoke();
    }
}
