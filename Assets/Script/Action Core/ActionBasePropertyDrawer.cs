using System;
using System.Collections.Generic;
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
            // دکمه "Add Action"
            if (GUI.Button(position, "Add Action"))
            {
                GenericMenu menu = new GenericMenu();
                var actionTypes = GetAllActionTypes();

                foreach (var type in actionTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        property.serializedObject.Update();
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }
        }
        else
        {
            // عنوان اکشن
            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            GUI.Box(headerRect, GUIContent.none, EditorStyles.helpBox);
            EditorGUI.LabelField(headerRect, property.managedReferenceValue.GetType().Name, EditorStyles.boldLabel);

            // نمایش فیلدهای اکشن
            var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 4, position.width, position.height - EditorGUIUtility.singleLineHeight - 8);
            EditorGUI.PropertyField(contentRect, property, true);

            // دکمه‌های حذف و تغییر
            var buttonWidth = 50;
            var buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(buttonRect, "Remove"))
            {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUI.EndProperty();
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
