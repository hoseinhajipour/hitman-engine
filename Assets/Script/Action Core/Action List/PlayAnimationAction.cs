using UnityEngine;

[System.Serializable]
public class PlayAnimationAction : ActionBase
{
    public GameObject target;
    public string animationName;
    private string idleStateName = "idle"; // The name of the Idle state in the animator
    [Tooltip("Wait for the animation to finish before invoking onComplete.")]
    public bool waitForEnd = false;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("PlayAnimationAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        Animator animator = target.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayAnimationAction: No Animator component found on target!");
            onComplete?.Invoke();
            return;
        }

        // Play the animation
        animator.Play(animationName);

        if (waitForEnd)
        {
            // Start a coroutine to wait for the animation to finish
            target.GetComponent<MonoBehaviour>()?.StartCoroutine(WaitForAnimationCoroutine(animator, onComplete));
        }
        else
        {
            // Immediately invoke the completion callback
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitForAnimationCoroutine(Animator animator, System.Action onComplete)
    {
        // Wait until the Animator has transitioned to the target animation state
        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash(animationName))
        {
            yield return null;
        }

        // Wait until the animation finishes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Return to the Idle state after the animation finishes
        animator.Play(idleStateName);

        // Invoke the completion callback
        onComplete?.Invoke();
    }
}
