using UnityEngine;

[System.Serializable]
public class GuardActiveAction : ActionBase
{
    public GameObject target;
    public bool GuardMode;
    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("GuardActiveAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        var perceptionComponent = target.GetComponent<Perception>();
        if (perceptionComponent == null)
        {
            Debug.LogError("GuardActiveAction: Target does not have a Perception component!");
            onComplete?.Invoke();
            return;
        }

        // Set the isGuardMode to true
        perceptionComponent.isGuardMode = GuardMode;
        Debug.Log("GuardActiveAction: isGuardMode set to true.");
        onComplete?.Invoke();
    }
}
