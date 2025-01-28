using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeReference]
    public List<ActionBase> actions = new List<ActionBase>();

    private bool isRunning = false; // Track if actions are currently running

    [Tooltip("Destroy the target GameObject after all actions are complete.")]
    public bool destroyOnEnd = false;

    [Tooltip("Enable detailed debugging logs.")]
    public bool enableDebugLogs = false;

    // Event triggered when all actions are complete
    public event System.Action OnActionsComplete;

    public bool IsRunning()
    {
        return isRunning;
    }

    public void RunActions(GameObject target)
    {
        if (isRunning)
        {
            Log("ActionManager: Actions are already running. Ignoring RunActions call.");
            return;
        }
        StartCoroutine(RunActionsSequentially(target));
    }

    public void ResetActions(GameObject target)
    {
        if (isRunning)
        {
            Log("ActionManager: Actions are currently running. Stopping before reset.");
            StopActions();
        }
        StartCoroutine(RunActionsSequentially(target));
        Log("ActionManager: Actions reset.");
    }

    public void StopActions()
    {
        StopAllCoroutines();
        foreach (var action in actions)
        {
            if (action is ICancelableAction cancelableAction)
            {
                cancelableAction.Cancel();
            }
        }
        isRunning = false;
        Log("ActionManager: Actions stopped.");
    }

    private System.Collections.IEnumerator RunActionsSequentially(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("ActionManager: Target GameObject is null!");
            yield break;
        }

        isRunning = true;

        foreach (var action in actions)
        {
            if (action == null)
            {
                Log("ActionManager: Null action detected in the list, skipping.");
                continue;
            }

            bool isComplete = false;
            Log($"Executing action: {action.GetType().Name} on target: {target.name}");

            action.Execute(target, () => isComplete = true);

            // Wait until the action completes
            yield return new WaitUntil(() => isComplete);
        }

        isRunning = false; // Mark actions as complete

        if (destroyOnEnd && target != null)
        {
            Log($"ActionManager: Destroying target GameObject: {target.name}");
            Destroy(target);
        }

        // Trigger the OnActionsComplete event
        OnActionsComplete?.Invoke();
        Log("ActionManager: All actions completed.");
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(message);
        }
    }
}

// Optional interface for cancelable actions
public interface ICancelableAction
{
    void Cancel();
}