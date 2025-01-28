using UnityEngine;

[System.Serializable]
public class ConditionAction : ActionBase
{
    [Tooltip("The target GameObject containing the component.")]
    public GameObject target;

    [Tooltip("The name of the component to inspect.")]
    public string componentName;

    [Tooltip("The name of the parameter to compare.")]
    public string parameterName;

    [Tooltip("The expected value to compare against.")]
    public string expectedValue;

    [Tooltip("Comparison operator: Equals, NotEquals, GreaterThan, LessThan.")]
    public ComparisonOperator comparisonOperator;

    [Tooltip("ActionManager to execute if the condition is met.")]
    public ActionManager SuccessAction;

    [Tooltip("ActionManager to execute if the condition is not met.")]
    public ActionManager FailureAction;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ConditionAction: Target GameObject is null!");
            FailureAction?.RunActions(target); // Execute failure actions if target is null
            onComplete?.Invoke();
            return;
        }

        var component = target.GetComponent(componentName);
        if (component == null)
        {
            Debug.LogError($"ConditionAction: Component '{componentName}' not found on target GameObject.");
            FailureAction?.RunActions(target); // Execute failure actions if component not found
            onComplete?.Invoke();
            return;
        }

        var parameter = component.GetType().GetProperty(parameterName);
        if (parameter == null)
        {
            Debug.LogError($"ConditionAction: Parameter '{parameterName}' not found in component '{componentName}'.");
            FailureAction?.RunActions(target); // Execute failure actions if parameter not found
            onComplete?.Invoke();
            return;
        }

        var parameterValue = parameter.GetValue(component)?.ToString();
        if (parameterValue == null)
        {
            Debug.LogError($"ConditionAction: Parameter '{parameterName}' is null or cannot be retrieved.");
            FailureAction?.RunActions(target); // Execute failure actions if parameter value is null
            onComplete?.Invoke();
            return;
        }

        bool conditionMet = false;

        switch (comparisonOperator)
        {
            case ComparisonOperator.Equals:
                conditionMet = parameterValue == expectedValue;
                break;
            case ComparisonOperator.NotEquals:
                conditionMet = parameterValue != expectedValue;
                break;
            case ComparisonOperator.GreaterThan:
                conditionMet = float.TryParse(parameterValue, out float paramValue) &&
                               float.TryParse(expectedValue, out float expValue) &&
                               paramValue > expValue;
                break;
            case ComparisonOperator.LessThan:
                conditionMet = float.TryParse(parameterValue, out float paramVal) &&
                               float.TryParse(expectedValue, out float expVal) &&
                               paramVal < expVal;
                break;
        }

        if (conditionMet)
        {
            Debug.Log("ConditionAction: Condition met!");
            SuccessAction?.RunActions(target); // Execute success actions if condition is met
        }
        else
        {
            Debug.LogWarning("ConditionAction: Condition not met.");
            FailureAction?.RunActions(target); // Execute failure actions if condition is not met
        }

        onComplete?.Invoke();
    }
}

public enum ComparisonOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan
}
