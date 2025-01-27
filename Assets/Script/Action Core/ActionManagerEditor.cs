using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionManager))]
public class ActionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ActionManager manager = (ActionManager)target;
        if (GUILayout.Button("Run Actions"))
        {
            if (Application.isPlaying)
            {
                manager.RunActions(manager.gameObject);
            }
            else
            {
                Debug.LogWarning("You can only run actions in Play Mode.");
            }
        }
    }
}
