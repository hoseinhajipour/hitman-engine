// Runtime/AIBehaviorFSM.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Task,
    Selector,
    Sequence,
    Repeater,
    RandomSelector,
    RandomSequence
}

[Serializable]
public class Node
{
    public string Name;
    public Rect Position; // Position in the editor window
    public List<Transition> Transitions = new List<Transition>();
    public NodeType Type;

    public Node(string name, Rect position, NodeType type)
    {
        Name = name;
        Position = position;
        Type = type;
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
        // Example default nodes
    }

    public void ExecuteNode(Node node)
    {
        switch (node.Type)
        {
            case NodeType.Task:
                ExecuteTask(node);
                break;

            case NodeType.Selector:
                ExecuteSelector(node);
                break;

            case NodeType.Sequence:
                ExecuteSequence(node);
                break;

            case NodeType.Repeater:
                ExecuteRepeater(node);
                break;

            case NodeType.RandomSelector:
                ExecuteRandomSelector(node);
                break;

            case NodeType.RandomSequence:
                ExecuteRandomSequence(node);
                break;

            default:
                Debug.LogError($"Unknown NodeType: {node.Type}");
                break;
        }
    }

    private void ExecuteTask(Node node)
    {
        Debug.Log($"Executing Task Node: {node.Name}");
        // Add task-specific logic here
    }

    private void ExecuteSelector(Node node)
    {
        Debug.Log($"Executing Selector Node: {node.Name}");
        foreach (var transition in node.Transitions)
        {
            if (EvaluateCondition(transition))
            {
                ExecuteNode(transition.To);
                break;
            }
        }
    }

    private void ExecuteSequence(Node node)
    {
        Debug.Log($"Executing Sequence Node: {node.Name}");
        foreach (var transition in node.Transitions)
        {
            if (!EvaluateCondition(transition))
            {
                return; // Stop if any condition fails
            }
            ExecuteNode(transition.To);
        }
    }

    private void ExecuteRepeater(Node node)
    {
        Debug.Log($"Executing Repeater Node: {node.Name}");
        foreach (var transition in node.Transitions)
        {
            while (EvaluateCondition(transition))
            {
                ExecuteNode(transition.To);
            }
        }
    }

    private void ExecuteRandomSelector(Node node)
    {
        Debug.Log($"Executing Random Selector Node: {node.Name}");
        if (node.Transitions.Count == 0) return;
        var randomTransition = node.Transitions[UnityEngine.Random.Range(0, node.Transitions.Count)];
        if (EvaluateCondition(randomTransition))
        {
            ExecuteNode(randomTransition.To);
        }
    }

    private void ExecuteRandomSequence(Node node)
    {
        Debug.Log($"Executing Random Sequence Node: {node.Name}");
        var shuffledTransitions = new List<Transition>(node.Transitions);
        Shuffle(shuffledTransitions);
        foreach (var transition in shuffledTransitions)
        {
            if (!EvaluateCondition(transition))
            {
                return; // Stop if any condition fails
            }
            ExecuteNode(transition.To);
        }
    }

    private bool EvaluateCondition(Transition transition)
    {
        // Placeholder for evaluating conditions on a transition
        return true; // Assume all conditions pass by default
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
