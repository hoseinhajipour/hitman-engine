using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionTrigger))]
public class ActionTriggerEditor : Editor
{
    private SerializedProperty actionManager;
    private SerializedProperty targetObject;
    private SerializedProperty triggerType;
    private SerializedProperty triggerKey;
    private SerializedProperty triggerTag;

    private void OnEnable()
    {
        // Ensure the target object is valid
        if (target == null)
        {
            Debug.LogError("ActionTrigger object is null.");
            return;
        }

        // Find the serialized properties
        actionManager = serializedObject.FindProperty("actionManager");
        targetObject = serializedObject.FindProperty("targetObject");
        triggerType = serializedObject.FindProperty("triggerType");
        triggerKey = serializedObject.FindProperty("triggerKey");
        triggerTag = serializedObject.FindProperty("triggerTag");

        // Null checks for better error handling
        if (actionManager == null) Debug.LogError("actionManager is not assigned in the inspector.");
        if (targetObject == null) Debug.LogError("targetObject is not assigned in the inspector.");
        if (triggerType == null) Debug.LogError("triggerType is not assigned in the inspector.");
        if (triggerKey == null) Debug.LogError("triggerKey is not assigned in the inspector.");
        if (triggerTag == null) Debug.LogError("triggerTag is not assigned in the inspector.");
    }

    public override void OnInspectorGUI()
    {
        // Check if the target object is valid
        if (target == null)
        {
            EditorGUILayout.HelpBox("Target object is null. Ensure the ActionTrigger component is attached to a GameObject.", MessageType.Error);
            return;
        }

        // Start modifying the inspector
        serializedObject.Update();

        // Display common fields
        EditorGUILayout.PropertyField(actionManager);
        EditorGUILayout.PropertyField(targetObject);

        // Display trigger type field
        EditorGUILayout.PropertyField(triggerType);

        // Display additional fields based on the selected trigger type
        TriggerType currentTriggerType = (TriggerType)triggerType.enumValueIndex;
        switch (currentTriggerType)
        {
            case TriggerType.OnKeyPress:
                // Show Key field for OnKeyPress trigger
                EditorGUILayout.PropertyField(triggerKey, new GUIContent("Key"));
                break;

            case TriggerType.OnMouseClick:
                // No additional field for OnMouseClick
                break;

            case TriggerType.OnTag:
                // Show Tag field for OnTag trigger
                EditorGUILayout.PropertyField(triggerTag, new GUIContent("Tag"));
                break;

            case TriggerType.OnTriggerEnter:
                // No additional field for OnTriggerEnter
                break;

            default:
                Debug.LogWarning("Unknown TriggerType selected.");
                break;
        }

        // Apply changes to serialized properties
        serializedObject.ApplyModifiedProperties();
    }
}
