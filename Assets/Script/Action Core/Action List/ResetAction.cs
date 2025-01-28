using UnityEngine;
[ActionCategory("Utility")]
[System.Serializable]
public class ResetAction : ActionBase
{
    public ActionManager targetActionManager; // The ActionManager to reset and restart

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (targetActionManager == null)
        {
            Debug.LogError("ResetAction: Target ActionManager is null!");
            onComplete?.Invoke();
            return;
        }

        Debug.Log("ResetAction: Resetting ActionManager.");
        targetActionManager.ResetActions(hit_target);

        Debug.Log("ResetAction: Completed reset.");
        onComplete?.Invoke();
    }

}
