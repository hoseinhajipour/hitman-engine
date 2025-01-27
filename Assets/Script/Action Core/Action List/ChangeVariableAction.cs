using UnityEngine;

[System.Serializable]
public class ChangeVariableAction : ActionBase
{
    public GameObject target;
    public string componentName;
    public string variableName;
    public float newValue;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ChangeVariableAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        var component = target.GetComponent(componentName);
        if (component == null)
        {
            Debug.LogError($"ChangeVariableAction: Component {componentName} not found on target!");
            onComplete?.Invoke();
            return;
        }

        var field = component.GetType().GetField(variableName);
        if (field != null)
        {
            field.SetValue(component, newValue);
        }
        else
        {
            Debug.LogError($"ChangeVariableAction: Field {variableName} not found in component {componentName}!");
        }

        onComplete?.Invoke();
    }
}
