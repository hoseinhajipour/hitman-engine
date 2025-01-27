using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActionSearchProvider : ScriptableObject, ISearchWindowProvider
{
    private Action<Type> onTypeSelected;

    public static void ShowSearchWindow(Action<Type> onTypeSelected)
    {
        var searchProvider = ScriptableObject.CreateInstance<ActionSearchProvider>();
        searchProvider.onTypeSelected = onTypeSelected;

        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchProvider);
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Actions"), 0)
        };

        var actionsWithPath = GetAllActionTypesWithPath();

        // Dictionary to track created groups
        var groupEntries = new Dictionary<string, SearchTreeGroupEntry>();

        foreach (var (type, path) in actionsWithPath)
        {
            var parts = path.Split('/'); // Split path into folders
            var currentPath = "";
            SearchTreeGroupEntry parentGroup = null;

            // Create or reuse groups
            for (int i = 0; i < parts.Length - 1; i++)
            {
                currentPath = string.Join("/", parts.Take(i + 1));
                if (!groupEntries.ContainsKey(currentPath))
                {
                    var newGroup = new SearchTreeGroupEntry(new GUIContent(parts[i]), i + 1);
                    tree.Add(newGroup);
                    groupEntries[currentPath] = newGroup;
                }

                parentGroup = groupEntries[currentPath];
            }

            // Add the action to the final group
            tree.Add(new SearchTreeEntry(new GUIContent(parts[^1]))
            {
                level = parts.Length,
                userData = type
            });
        }

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
    {
        if (entry.userData is Type selectedType)
        {
            onTypeSelected?.Invoke(selectedType);
            return true;
        }
        return false;
    }

    private static List<(Type type, string path)> GetAllActionTypesWithPath()
    {
        var actionBaseType = typeof(ActionBase);
        var actionsWithPath = new List<(Type type, string path)>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (actionBaseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    // Check for ActionCategory attribute
                    var attributes = type.GetCustomAttributes(typeof(ActionCategoryAttribute), false);
                    string path = attributes.Length > 0
                        ? ((ActionCategoryAttribute)attributes[0]).Category.Trim() // Trim any leading/trailing spaces
                        : "Uncategorized";

                    actionsWithPath.Add((type, $"{path}/{type.Name}"));
                }
            }
        }

        return actionsWithPath;
    }
}
