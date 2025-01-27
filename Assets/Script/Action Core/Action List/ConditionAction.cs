using UnityEngine;

[System.Serializable]
public class ConditionAction : ActionBase
{
    public GameObject target; // Target GameObject
    public string componentName; // Name of the component to inspect
    public string variableName; // Name of the variable to compare

    public enum ComparisonType
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

    public ComparisonType comparisonType; // Type of comparison
    public string compareValue; // The value to compare (string to support all types)

    [SerializeReference]
    public ActionBase[] actionsOnSuccess; // Actions to execute if the condition is true

    [SerializeReference]
    public ActionBase[] actionsOnFailure; // Actions to execute if the condition is false

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ConditionAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        var component = target.GetComponent(componentName);
        if (component == null)
        {
            Debug.LogError($"ConditionAction: Component {componentName} not found on target!");
            onComplete?.Invoke();
            return;
        }

        var field = component.GetType().GetField(variableName);
        if (field == null)
        {
            Debug.LogError($"ConditionAction: Field {variableName} not found in component {componentName}!");
            onComplete?.Invoke();
            return;
        }

        object value = field.GetValue(component);

        bool conditionMet = false;

        if (value is int intValue)
        {
            conditionMet = CompareInt(intValue);
        }
        else if (value is bool boolValue)
        {
            conditionMet = CompareBool(boolValue);
        }
        else if (value is string stringValue)
        {
            conditionMet = CompareString(stringValue);
        }
        else
        {
            Debug.LogError("ConditionAction: Unsupported variable type for comparison!");
            onComplete?.Invoke();
            return;
        }

        ExecuteActions(conditionMet ? actionsOnSuccess : actionsOnFailure, hit_target, onComplete);
    }

    private bool CompareInt(int value)
    {
        if (int.TryParse(compareValue, out int compareTo))
        {
            switch (comparisonType)
            {
                case ComparisonType.Equals: return value == compareTo;
                case ComparisonType.NotEquals: return value != compareTo;
                case ComparisonType.GreaterThan: return value > compareTo;
                case ComparisonType.LessThan: return value < compareTo;
                case ComparisonType.GreaterThanOrEqual: return value >= compareTo;
                case ComparisonType.LessThanOrEqual: return value <= compareTo;
            }
        }
        Debug.LogError("ConditionAction: Invalid comparison value for int!");
        return false;
    }

    private bool CompareBool(bool value)
    {
        if (bool.TryParse(compareValue, out bool compareTo))
        {
            switch (comparisonType)
            {
                case ComparisonType.Equals: return value == compareTo;
                case ComparisonType.NotEquals: return value != compareTo;
                default:
                    Debug.LogError("ConditionAction: Invalid comparison type for bool!");
                    return false;
            }
        }
        Debug.LogError("ConditionAction: Invalid comparison value for bool!");
        return false;
    }

    private bool CompareString(string value)
    {
        switch (comparisonType)
        {
            case ComparisonType.Equals: return value == compareValue;
            case ComparisonType.NotEquals: return value != compareValue;
            default:
                Debug.LogError("ConditionAction: Invalid comparison type for string!");
                return false;
        }
    }

    private void ExecuteActions(ActionBase[] actions, GameObject target, System.Action onComplete)
    {
        if (actions == null || actions.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        var actionManager = target.GetComponent<ActionManager>();
        if (actionManager == null)
        {
            Debug.LogError("ConditionAction: ActionManager component not found on target!");
            onComplete?.Invoke();
            return;
        }

        actionManager.StartCoroutine(RunActions(actions, target, onComplete));
    }

    private System.Collections.IEnumerator RunActions(ActionBase[] actions, GameObject target, System.Action onComplete)
    {
        foreach (var action in actions)
        {
            bool isComplete = false;
            action.Execute(target, () => isComplete = true);

            yield return new WaitUntil(() => isComplete);
        }

        onComplete?.Invoke();
    }
}
