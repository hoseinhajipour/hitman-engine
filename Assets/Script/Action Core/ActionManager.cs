using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeReference]
    public List<ActionBase> actions = new List<ActionBase>();

    public void RunActions(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("ActionManager: Target GameObject is null!");
            return;
        }

        StartCoroutine(RunActionsSequentially(target));
    }


    private System.Collections.IEnumerator RunActionsSequentially(GameObject target)
    {
        foreach (var action in actions)
        {
            if (action == null)
            {
                Debug.LogWarning("ActionManager: Null action detected in the list, skipping.");
                continue;
            }

            bool isComplete = false;

            Debug.Log($"Executing action: {action.GetType().Name} on target: {target?.name}");

            action.Execute(target, () => isComplete = true);

            yield return new WaitUntil(() => isComplete);
        }
    }


}
