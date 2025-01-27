using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    [SerializeReference]
    public List<ActionBase> actions = new List<ActionBase>();

    public void RunActions(GameObject target)
    {
        foreach (var action in actions)
        {
            action?.Execute(target);
        }
    }
}
