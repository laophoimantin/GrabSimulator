using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    // Events for the UI 
    public event Action<DialogueNode> OnNodeStart;
    
    private Action _onDialogueFinishedCallback;
    
    public event Action OnDialogueEnd;

    private Dictionary<string, DialogueNode> _nodeMap;
    private DialogueNode _currentNode;
    private bool _isDialogueActive;

    public void StartDialogue(DialogueDataSO data, Action onFinishedEvent = null)
    {
        if (_isDialogueActive) return;
        
        string startNodeID = DefaultNodeIDs.StartNodeID;
        _onDialogueFinishedCallback = onFinishedEvent;
        _nodeMap = new Dictionary<string, DialogueNode>();
        foreach (var node in data.Nodes)
        {
            if (!_nodeMap.ContainsKey(node.NodeID))
                _nodeMap.Add(node.NodeID, node);
            else
                Debug.LogWarning($"Duplicate Node ID found: {node.NodeID}");
        }

        _isDialogueActive = true;
        CursorLocker.RequestCursor(this);
        
        InputManager.Instance.SetDialogueStateMode();
        
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

   private void JumpToNode(string id)
    {
        if (_nodeMap.TryGetValue(id, out DialogueNode nextNode))
        {
            _currentNode = nextNode;
            
            DialogueNode localNode = _currentNode;

            bool isDeadEnd = string.IsNullOrWhiteSpace(localNode.Text) 
                          && string.IsNullOrEmpty(localNode.DefaultNextNodeID) 
                          && (localNode.Choices == null || localNode.Choices.Count == 0);

            if (isDeadEnd)
            {
                EndDialogue(); 
                
                if (localNode.EventTrigger != null)
                {
                    localNode.EventTrigger.Raise(); 
                }
                
                return; 
            }

            if (localNode.EventTrigger != null)
            {
                localNode.EventTrigger.Raise();
            }

            if (_currentNode != localNode) return; 

            if (string.IsNullOrWhiteSpace(localNode.Text))
            {
                Next(); 
            }
            else
            {
                OnNodeStart?.Invoke(localNode); 
            }
        }
        else
        {
            Debug.LogWarning($"Node ID '{id}' not found. Ending dialogue.");
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        CursorLocker.ReleaseCursor(this);
        InputManager.Instance.RestorePreviousGameplayState();
        
        _isDialogueActive = false;
        _currentNode = null;
        OnDialogueEnd?.Invoke();
        
        _onDialogueFinishedCallback?.Invoke();
        _onDialogueFinishedCallback = null;
    }
}