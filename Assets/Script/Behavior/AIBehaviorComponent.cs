// Runtime/AIBehaviorComponent.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviorComponent : MonoBehaviour
{
    public AIBehaviorFSM fsm;
    private Node currentNode;

    private void Start()
    {
        fsm = GetComponent<AIBehaviorFSM>();

        if (fsm.Nodes.Count > 0)
        {
            currentNode = fsm.Nodes[0]; // Start with the first node
            ExecuteNode(currentNode);
        }
        else
        {
            Debug.LogWarning("AIBehaviorFSM has no nodes assigned!");
        }
    }

    private void ExecuteNode(Node node)
    {
        if (node == null) return;

        Debug.Log($"Executing Node: {node.Name}");

        switch (node.Type)
        {
            case NodeType.Task:
                StartCoroutine(ExecuteTaskNode(node));
                break;

            case NodeType.Selector:
                StartCoroutine(ExecuteSelectorNode(node));
                break;

            case NodeType.Sequence:
                StartCoroutine(ExecuteSequenceNode(node));
                break;

            case NodeType.Repeater:
                StartCoroutine(ExecuteRepeaterNode(node));
                break;

            case NodeType.RandomSelector:
                StartCoroutine(ExecuteRandomSelectorNode(node));
                break;

            case NodeType.RandomSequence:
                StartCoroutine(ExecuteRandomSequenceNode(node));
                break;
        }
    }

    private IEnumerator ExecuteTaskNode(Node node)
    {
        // Simulate task execution (replace with actual task logic)
        Debug.Log($"Performing Task: {node.Name}");
        yield return new WaitForSeconds(1f);
        TransitionToNextNode(node);
    }

    private IEnumerator ExecuteSelectorNode(Node node)
    {
        foreach (var transition in node.Transitions)
        {
            if (EvaluateCondition(transition))
            {
                ExecuteNode(transition.To);
                yield break;
            }
        }
        Debug.Log($"Selector Node {node.Name} failed.");
    }

    private IEnumerator ExecuteSequenceNode(Node node)
    {
        foreach (var transition in node.Transitions)
        {
            if (!EvaluateCondition(transition))
            {
                Debug.Log($"Sequence Node {node.Name} failed.");
                yield break;
            }
            ExecuteNode(transition.To);
            yield return new WaitForSeconds(1f); // Simulate time between nodes
        }
    }

    private IEnumerator ExecuteRepeaterNode(Node node)
    {
        while (true)
        {
            foreach (var transition in node.Transitions)
            {
                if (EvaluateCondition(transition))
                {
                    ExecuteNode(transition.To);
                }
            }
            yield return null;
        }
    }

    private IEnumerator ExecuteRandomSelectorNode(Node node)
    {
        if (node.Transitions.Count == 0) yield break;

        var randomTransition = node.Transitions[Random.Range(0, node.Transitions.Count)];
        if (EvaluateCondition(randomTransition))
        {
            ExecuteNode(randomTransition.To);
        }
        yield return null;
    }

    private IEnumerator ExecuteRandomSequenceNode(Node node)
    {
        var shuffledTransitions = new List<Transition>(node.Transitions);
        Shuffle(shuffledTransitions);

        foreach (var transition in shuffledTransitions)
        {
            if (!EvaluateCondition(transition))
            {
                Debug.Log($"Random Sequence Node {node.Name} failed.");
                yield break;
            }
            ExecuteNode(transition.To);
            yield return new WaitForSeconds(1f);
        }
    }

    private void TransitionToNextNode(Node node)
    {
        if (node.Transitions.Count > 0)
        {
            ExecuteNode(node.Transitions[0].To);
        }
        else
        {
            Debug.Log($"Node {node.Name} has no outgoing transitions.");
        }
    }

    private bool EvaluateCondition(Transition transition)
    {
        // Placeholder for transition conditions
        return true; // Assume all conditions pass by default
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
