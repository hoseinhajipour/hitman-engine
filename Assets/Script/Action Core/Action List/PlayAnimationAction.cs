using UnityEngine;

[System.Serializable]
public class PlayAnimationAction : ActionBase
{
    public GameObject target;
    public string animationName;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("PlayAnimationAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        Animator animator = target.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayAnimationAction: No Animator component found on target!");
            onComplete?.Invoke();
            return;
        }

        animator.Play(animationName);
        onComplete?.Invoke();
    }
}
