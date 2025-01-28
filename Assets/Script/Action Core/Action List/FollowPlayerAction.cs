using UnityEngine;
using UnityEngine.AI;

[ActionCategory("Character")]
[System.Serializable]
public class FollowPlayerAction : ActionBase
{
    private GameObject Player;
    public GameObject Target;

    [Tooltip("The speed at which the NavMeshAgent should move.")]
    public float speed = 3.5f;

    [Tooltip("Enable running animation if the target has an Animator component.")]
    public bool Run;

    [Tooltip("Minimum distance to maintain from the player.")]
    public float followDistance = 2.0f;

    public float stopThreshold = 0.1f; // Threshold to stop the agent near the target

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("FollowPlayerAction: Target Player GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        NavMeshAgent agent = Target.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("FollowPlayerAction: No NavMeshAgent component found on the target GameObject!");
            onComplete?.Invoke();
            return;
        }

        // Set the agent's speed
        agent.speed = speed;

        // Handle Animator for running animation
        CharacterControllerWithNavMesh characterController = Target.GetComponent<CharacterControllerWithNavMesh>();
        if (characterController != null && Run)
        {
            characterController.isRunning = true;
        }

        // Start following the player
        hit_target.GetComponent<MonoBehaviour>()?.StartCoroutine(FollowPlayer(agent, characterController, onComplete));
    }

    private System.Collections.IEnumerator FollowPlayer(NavMeshAgent agent, CharacterControllerWithNavMesh characterController, System.Action onComplete)
    {
        while (Player != null && Vector3.Distance(agent.transform.position, Player.transform.position) > stopThreshold)
        {
            Vector3 direction = Player.transform.position - agent.transform.position;

            // Stop moving if within the follow distance
            if (direction.magnitude > followDistance)
            {
                agent.SetDestination(Player.transform.position);
            }
            else
            {
                agent.ResetPath();
            }

            yield return null; // Wait for the next frame
        }

        // Reset the running animation if applicable
        if (characterController != null && Run)
        {
            characterController.isRunning = false;
        }

        onComplete?.Invoke();
    }
}