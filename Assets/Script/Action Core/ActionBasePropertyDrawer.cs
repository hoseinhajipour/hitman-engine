using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionBase), true)]
public class ActionBasePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight; // ارتفاع برای دکمه "Add Action"

        // محاسبه ارتفاع کل اکشن
        return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight * 1.5f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.managedReferenceValue == null)
        {
            // "Add Action" button
            if (GUI.Button(position, "Add Action"))
            {
                // Open a custom search window
                ActionSearchProvider.ShowSearchWindow((selectedType) =>
                {
                    property.serializedObject.Update();
                    property.managedReferenceValue = Activator.CreateInstance(selectedType);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }
        else
        {
            // Header and label
            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            GUI.Box(headerRect, GUIContent.none, EditorStyles.helpBox);
            EditorGUI.LabelField(headerRect, property.managedReferenceValue.GetType().Name, EditorStyles.boldLabel);

            // Fields for the action
            var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 4, position.width, position.height - EditorGUIUtility.singleLineHeight - 8);
            EditorGUI.PropertyField(contentRect, property, true);

            // Buttons (Remove and Duplicate)
            var buttonWidth = 50;
            var removeButtonRect = new Rect(position.x + position.width - buttonWidth * 2 - 4, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);
            var duplicateButtonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(removeButtonRect, "Remove"))
            {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(duplicateButtonRect, "Duplicate"))
            {
                property.serializedObject.Update();

                var originalAction = property.managedReferenceValue;
                if (originalAction != null)
                {
                    // Clone the object using MemberwiseClone
                    var clonedAction = CloneAction(originalAction);
                    var parentList = GetParentList(property);

                    if (parentList != null)
                    {
                        parentList.Insert(GetPropertyIndex(property), clonedAction);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }

    private object CloneAction(object originalAction)
    {
        var type = originalAction.GetType();
        var clonedAction = Activator.CreateInstance(type);

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            field.SetValue(clonedAction, field.GetValue(originalAction));
        }

        return clonedAction;
    }

    private IList GetParentList(SerializedProperty property)
    {
        var path = property.propertyPath;
        var elements = path.Split('.');

        object currentObject = property.serializedObject.targetObject;

        // Traverse the path to find the parent list
        for (int i = 0; i < elements.Length - 1; i++)
        {
            var element = elements[i];
            if (element.Contains("Array"))
            {
                // Handle arrays/lists
                element = element.Replace("Array", "").Replace("data", "").Replace("[", "").Replace("]", "");
                if (int.TryParse(element, out int index) && currentObject is IList list)
                {
                    currentObject = list[index];
                }
            }
            else
            {
                var field = currentObject.GetType().GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    currentObject = field.GetValue(currentObject);
                }
            }
        }

        return currentObject as IList;
    }

    private int GetPropertyIndex(SerializedProperty property)
    {
        // Extract the index from the property path
        var match = Regex.Match(property.propertyPath, @"Array\.data\[(\d+)\]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
        {
            return index;
        }
        return -1;
    }

    private List<Type> GetAllActionTypes()
    {
        var actionBaseType = typeof(ActionBase);
        var types = new List<Type>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (actionBaseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    types.Add(type);
                }
            }
        }

        return types;
    }
}
