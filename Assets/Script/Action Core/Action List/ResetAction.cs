using UnityEngine;

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

        // Stop any ongoing actions in the ActionManager
        targetActionManager.StopAllCoroutines();

        // Restart the ActionManager
        targetActionManager.RunActions(hit_target);

        Debug.Log("ResetAction: ActionManager has been reset and restarted.");
        onComplete?.Invoke(); // Notify that the reset action is complete
    }
}
