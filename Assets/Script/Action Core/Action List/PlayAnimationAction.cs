using UnityEngine;

[System.Serializable]
public class PlayAnimationAction : ActionBase
{
    public GameObject target;
    public string animationName;
    [Tooltip("The layer index to apply the animation on. Set to -1 to ignore layers.")]
    public int avatarLayerIndex = -1; // Default to -1 to indicate no specific layer
    [Range(0f, 1f)]
    public float avatarLayerWeight = 1f; // Default weight for the specified layer
    [Tooltip("Wait for the animation to finish before invoking onComplete.")]
    public bool waitForEnd = false; // Checkbox for waiting for animation to end

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

        if (avatarLayerIndex >= 0 && avatarLayerIndex < animator.layerCount)
        {
            animator.SetLayerWeight(avatarLayerIndex, avatarLayerWeight);
        }

        animator.Play(animationName);

        if (waitForEnd)
        {
            AnimatorStateInfo animationStateInfo = animator.GetCurrentAnimatorStateInfo(avatarLayerIndex >= 0 ? avatarLayerIndex : 0);
            float animationLength = animationStateInfo.length;

            target.GetComponent<MonoBehaviour>()?.StartCoroutine(WaitForAnimationCoroutine(animationLength, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitForAnimationCoroutine(float duration, System.Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
}
