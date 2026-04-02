using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueDataSO : ScriptableObject
{
    public List<DialogueNode> Nodes;
}

[Serializable]
public class DialogueNode
{
    [Header("Node ID")]
    [Tooltip("Unique ID for this node")]
    public string NodeID;
    
    [Header("Content")]
    public string SpeakerName;
    [TextArea(3, 10)]
    public string Text; // One string per node

    [Header("Audio")]
    public AudioClip VoiceLine; // Later

    [Header("Branching")]
    [Tooltip("If empty, dialogue ends here.")]
    public List<DialogueChoice> Choices; 
    
    [Tooltip("If no choices, automatically jump to this Node ID after text finishes.")]
    public string DefaultNextNodeID;

    [Header("Events")]
    [Tooltip("Trigger a custom event when this node is reached. \nImprove later")]
    public DialogueGameEvent EventTrigger;
}

[Serializable]
public struct DialogueChoice
{
    public string ChoiceText;
    public string TargetNodeID;
}