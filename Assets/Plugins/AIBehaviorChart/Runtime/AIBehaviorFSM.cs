// Runtime/AIBehaviorFSM.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public string Name;
    public Rect Position; // Position in the editor window
    public List<Transition> Transitions = new List<Transition>();

    public Node(string name, Rect position)
    {
        Name = name;
        Position = position;
    }
}

[Serializable]
public class Transition
{
    public Node From;
    public Node To;

    public Transition(Node from, Node to)
    {
        From = from;
        To = to;
    }
}

[Serializable]
public class AIBehaviorFSM : ScriptableObject
{
    public List<Node> Nodes = new List<Node>();

    public void Initialize()
    {
        Nodes.Add(new Node("Idle", new Rect(10, 10, 100, 50)));
        Nodes.Add(new Node("Patrol", new Rect(150, 10, 100, 50)));
    }
}