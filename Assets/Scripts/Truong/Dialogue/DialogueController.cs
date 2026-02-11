using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    // Events for the UI 
    public event Action<DialogueNode> OnNodeStart;
    public event Action OnDialogueEnd;
    public event Action<string> OnCustomEventTriggered;

    private Dictionary<string, DialogueNode> _nodeMap;
    private DialogueNode _currentNode;
    private bool _isDialogueActive;

    public void StartDialogue(DialogueDataSO data, string startNodeID = "Start")
    {
        _nodeMap = new Dictionary<string, DialogueNode>();
        foreach (var node in data.Nodes)
        {
            if (!_nodeMap.ContainsKey(node.NodeID))
                _nodeMap.Add(node.NodeID, node);
            else
                Debug.LogWarning($"Duplicate Node ID found: {node.NodeID}");
        }

        _isDialogueActive = true;
        CursorManager.Instance.ShowCursor();
        JumpToNode(startNodeID);
    }

    public void SelectChoice(int choiceIndex)
    {
        if (!_isDialogueActive || _currentNode == null) return;

        if (choiceIndex < _currentNode.Choices.Count)
        {
            string targetID = _currentNode.Choices[choiceIndex].TargetNodeID;
            JumpToNode(targetID);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Next()
    {
        if (!_isDialogueActive) return;

        // The player must select a choice before moving on
        if (_currentNode.Choices != null && _currentNode.Choices.Count > 0) return;

        if (!string.IsNullOrEmpty(_currentNode.DefaultNextNodeID))
        {
            JumpToNode(_currentNode.DefaultNextNodeID);
        }
        else
        {
            EndDialogue();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void JumpToNode(string id)
    {
        if (_nodeMap.TryGetValue(id, out DialogueNode nextNode))
        {
            _currentNode = nextNode;
        
            if (_currentNode.EventTrigger != null)
            {
                _currentNode.EventTrigger.Raise();
            }

            OnNodeStart?.Invoke(_currentNode); // Update UI
        }
        else
        {
            Debug.LogWarning($"Node ID '{id}' not found. Ending dialogue.");
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        CursorManager.Instance.HideCursor();
        _isDialogueActive = false;
        _currentNode = null;
        OnDialogueEnd?.Invoke();
    }
}