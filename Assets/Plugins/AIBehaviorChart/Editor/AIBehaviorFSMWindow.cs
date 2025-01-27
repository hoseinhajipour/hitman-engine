// Editor/AIBehaviorFSMWindow.cs
using System;
using UnityEditor;
using UnityEngine;

public class AIBehaviorFSMWindow : EditorWindow
{
    private AIBehaviorFSM fsm;
    private Vector2 scrollPosition;
    private Node selectedNode;

    [MenuItem("Window/AI FSM Chart")]
    public static void ShowWindow()
    {
        GetWindow<AIBehaviorFSMWindow>("AI FSM Chart");
    }

    private void OnGUI()
    {
        if (fsm == null)
        {
            fsm = CreateInstance<AIBehaviorFSM>();
            fsm.Initialize();
        }

        // Scrollable area
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        // Draw transitions first (so they appear behind nodes)
        foreach (var node in fsm.Nodes)
        {
            foreach (var transition in node.Transitions)
            {
                DrawTransition(transition);
            }
        }

        // Draw nodes
        BeginWindows();
        for (int i = 0; i < fsm.Nodes.Count; i++)
        {
            fsm.Nodes[i].Position = GUI.Window(i, fsm.Nodes[i].Position, DrawNodeWindow, fsm.Nodes[i].Name);
        }
        EndWindows();

        GUILayout.EndScrollView();

        // Toolbar
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Add Node", EditorStyles.toolbarButton))
        {
            fsm.Nodes.Add(new Node("New State", new Rect(10, 10, 100, 50)));
        }
        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
        {
            SaveFSM();
        }
        if (GUILayout.Button("Load", EditorStyles.toolbarButton))
        {
            LoadFSM();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawNodeWindow(int id)
    {
        var node = fsm.Nodes[id];

        // Draw the node's name field
        node.Name = GUILayout.TextField(node.Name);

        // Button to add a transition from this node
        if (GUILayout.Button("Add Transition"))
        {
            Debug.Log($"Selected Node: {node.Name}");
            selectedNode = node;
        }

        if (selectedNode != null && selectedNode != node)
        {
            if (GUILayout.Button("Connect to Selected"))
            {
                Debug.Log($"Connecting {selectedNode.Name} to {node.Name}");
                selectedNode.Transitions.Add(new Transition(selectedNode, node));
                selectedNode = null;
            }
        }

        // Allow dragging the window
        GUI.DragWindow();
    }

    private void DrawTransition(Transition transition)
    {
        Vector2 start = transition.From.Position.center;
        Vector2 end = transition.To.Position.center;
        Handles.DrawBezier(start, end, start + Vector2.right * 50, end - Vector2.right * 50, Color.green, null, 2);
    }

    private void SaveFSM()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save FSM", "NewFSM", "asset", "Save");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(fsm, path);
            AssetDatabase.SaveAssets();
        }
    }

    private void LoadFSM()
    {
        string path = EditorUtility.OpenFilePanel("Load FSM", "Assets", "asset");
        if (!string.IsNullOrEmpty(path))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
            fsm = AssetDatabase.LoadAssetAtPath<AIBehaviorFSM>(path);
        }
    }
}