using UnityEngine;

[System.Serializable]
public class SetComponentParameterAction : ActionBase
{
    public GameObject target;
    public string componentName; // The name of the component to find
    public string parameterName; // The name of the parameter to set
    public string parameterValue; // The value to set

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("SetComponentParameterAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        // Find the component by name
        var component = target.GetComponent(componentName);
        if (component == null)
        {
            Debug.LogError($"SetComponentParameterAction: Component '{componentName}' not found on {target.name}.");
            onComplete?.Invoke();
            return;
        }

        // Find and set the parameter
        var field = component.GetType().GetField(parameterName);
        if (field != null)
        {
            field.SetValue(component, ConvertParameter(field.FieldType, parameterValue));
        }
        else
        {
            var property = component.GetType().GetProperty(parameterName);
            if (property != null)
            {
                property.SetValue(component, ConvertParameter(property.PropertyType, parameterValue));
            }
            else
            {
                Debug.LogError($"SetComponentParameterAction: Parameter '{parameterName}' not found in component '{componentName}'.");
            }
        }

        onComplete?.Invoke();
    }

    private object ConvertParameter(System.Type type, string value)
    {
        try
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(bool)) return bool.Parse(value);
            if (type == typeof(string)) return value;
            Debug.LogWarning($"SetComponentParameterAction: Unsupported parameter type '{type.Name}'. Returning the string value.");
            return value;
        }
        catch
        {
            Debug.LogError($"SetComponentParameterAction: Failed to convert value '{value}' to type '{type.Name}'.");
            return null;
        }
    }
}
