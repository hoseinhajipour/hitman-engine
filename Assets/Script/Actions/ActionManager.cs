using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeReference]
    public List<ActionBase> actions = new List<ActionBase>();

    public void RunActions(GameObject target)
    {
        StartCoroutine(RunActionsSequentially(target));
    }

    private System.Collections.IEnumerator RunActionsSequentially(GameObject target)
    {
        foreach (var action in actions)
        {
            bool isComplete = false;

            action.Execute(target, () => isComplete = true);

            // Wait until the action completes
            yield return new WaitUntil(() => isComplete);
        }
    }
}
