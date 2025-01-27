// Editor/AIBehaviorFSMWindow.cs
using System;
using UnityEditor;
using UnityEngine;

public class AIBehaviorFSMWindow : EditorWindow
{
    private AIBehaviorFSM fsm;
    private Vector2 scrollPosition;
    private Vector2 zoomCoordsOrigin = Vector2.zero; // Panning origin
    private float zoomScale = 1.0f;
    private Node selectedNode;
    private Node transitionStartNode;
    private Transition selectedTransition;
    private const float ZoomMin = 0.5f;
    private const float ZoomMax = 2.0f;
    private bool isCreatingTransition = false;
    private Vector2 currentMousePosition;

    [MenuItem("Window/AI FSM Editor")]
    public static void ShowWindow()
    {
        GetWindow<AIBehaviorFSMWindow>("AI FSM Editor");
    }

    private void OnGUI()
    {
        if (fsm == null)
        {
            fsm = CreateInstance<AIBehaviorFSM>();
            fsm.Initialize();
        }

        HandleZoomAndPan();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        DrawTransitions();

        BeginWindows();
        for (int i = 0; i < fsm.Nodes.Count; i++)
        {
            fsm.Nodes[i].Position = GUI.Window(i, fsm.Nodes[i].Position, DrawNodeWindow, fsm.Nodes[i].Name);
        }
        EndWindows();

        if (isCreatingTransition && transitionStartNode != null)
        {
            DrawTemporaryTransition();
        }

        GUILayout.EndScrollView();

        DrawToolbar();
        DrawInspector();

        HandleInputEvents();
    }

    private void HandleZoomAndPan()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            float delta = -e.delta.y / 150.0f;
            zoomScale = Mathf.Clamp(zoomScale + delta, ZoomMin, ZoomMax);
            e.Use();
        }

        if (e.type == EventType.MouseDrag && e.button == 2) // Middle mouse drag
        {
            zoomCoordsOrigin += e.delta;
            e.Use();
        }

        GUIUtility.ScaleAroundPivot(Vector2.one * zoomScale, zoomCoordsOrigin);
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Add Task Node", EditorStyles.toolbarButton))
        {
            AddNode(NodeType.Task);
        }
        if (GUILayout.Button("Add Selector Node", EditorStyles.toolbarButton))
        {
            AddNode(NodeType.Selector);
        }
        if (GUILayout.Button("Add Sequence Node", EditorStyles.toolbarButton))
        {
            AddNode(NodeType.Sequence);
        }
        if (GUILayout.Button("Save FSM", EditorStyles.toolbarButton))
        {
            SaveFSM();
        }
        if (GUILayout.Button("Load FSM", EditorStyles.toolbarButton))
        {
            LoadFSM();
        }

        GUILayout.EndHorizontal();
    }

    private void DrawInspector()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(200));

        if (selectedNode != null)
        {
            GUILayout.Label("Selected Node", EditorStyles.boldLabel);
            selectedNode.Name = EditorGUILayout.TextField("Name", selectedNode.Name);
            EditorGUILayout.LabelField("Type", selectedNode.Type.ToString());

            if (GUILayout.Button("Delete Node"))
            {
                DeleteNode(selectedNode);
            }
        }

        if (selectedTransition != null)
        {
            GUILayout.Label("Selected Transition", EditorStyles.boldLabel);
            GUILayout.Label($"From: {selectedTransition.From.Name}");
            GUILayout.Label($"To: {selectedTransition.To.Name}");

            if (GUILayout.Button("Delete Transition"))
            {
                DeleteTransition(selectedTransition);
            }
        }

        GUILayout.EndVertical();
    }

    private void AddNode(NodeType type)
    {
        fsm.Nodes.Add(new Node($"New {type} Node", new Rect(10, 10, 150, 75), type));
    }

    private void DrawNodeWindow(int id)
    {
        var node = fsm.Nodes[id];

        GUILayout.Label($"Type: {node.Type}", EditorStyles.boldLabel);

        node.Name = GUILayout.TextField(node.Name);

        if (GUILayout.Button("Add Transition"))
        {
            transitionStartNode = node;
            isCreatingTransition = true;
        }

        if (GUILayout.Button("Delete Node"))
        {
            DeleteNode(node);
            return;
        }

        GUI.DragWindow();
    }

    private void DrawTransitions()
    {
        foreach (var node in fsm.Nodes)
        {
            foreach (var transition in node.Transitions)
            {
                DrawTransition(transition);
            }
        }
    }

    private void DrawTransition(Transition transition)
    {
        Vector2 start = transition.From.Position.center;
        Vector2 end = transition.To.Position.center;
        Handles.DrawBezier(start, end, start + Vector2.right * 50, end - Vector2.right * 50, Color.white, null, 2);

        if (Handles.Button((start + end) * 0.5f, Quaternion.identity, 5, 10, Handles.CircleHandleCap))
        {
            selectedTransition = transition;
        }
    }

    private void DrawTemporaryTransition()
    {
        Vector2 start = transitionStartNode.Position.center;
        Vector2 end = currentMousePosition;
        Handles.DrawBezier(start, end, start + Vector2.right * 50, end - Vector2.right * 50, Color.yellow, null, 2);
    }

    private void HandleInputEvents()
    {
        Event e = Event.current;

        if (isCreatingTransition && e.type == EventType.MouseMove)
        {
            currentMousePosition = e.mousePosition;
            Repaint();
        }

        if (isCreatingTransition && e.type == EventType.MouseUp && e.button == 0)
        {
            Node targetNode = GetNodeUnderMouse(e.mousePosition);

            if (targetNode != null && targetNode != transitionStartNode)
            {
                transitionStartNode.Transitions.Add(new Transition(transitionStartNode, targetNode));
            }

            isCreatingTransition = false;
            transitionStartNode = null;
        }
    }

    private Node GetNodeUnderMouse(Vector2 mousePosition)
    {
        foreach (var node in fsm.Nodes)
        {
            if (node.Position.Contains(mousePosition))
            {
                return node;
            }
        }

        return null;
    }

    private void DeleteNode(Node node)
    {
        foreach (var n in fsm.Nodes)
        {
            n.Transitions.RemoveAll(t => t.To == node);
        }

        fsm.Nodes.Remove(node);
    }

    private void DeleteTransition(Transition transition)
    {
        transition.From.Transitions.Remove(transition);
        selectedTransition = null;
    }

    private void SaveFSM()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save FSM", "NewFSM", "asset", "Save your FSM asset");
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
