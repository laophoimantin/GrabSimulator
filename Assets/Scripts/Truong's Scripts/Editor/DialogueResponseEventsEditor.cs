using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueResponseEvents))]
public class DialogueResponseEventsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DialogueResponseEvents dialogueResponseEvents = (DialogueResponseEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            dialogueResponseEvents.OnValidate();
        }
    }
}
