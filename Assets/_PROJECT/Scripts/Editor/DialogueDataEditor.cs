using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueDataSO))]
public class DialogueDataEditor : Editor
{
    private ReorderableList _reorderableList;
    private SerializedProperty _nodesProperty;

    private void OnEnable()
    {
        _nodesProperty = serializedObject.FindProperty("Nodes");

        _reorderableList = new ReorderableList(serializedObject, _nodesProperty, true, true, true, true);

        // 1. Draw the Header
        _reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Dialogue Nodes");
        };

        // 2. Draw Each Element
        _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _nodesProperty.GetArrayElementAtIndex(index);

            // Get Properties
            var nodeID = element.FindPropertyRelative("NodeID");
            var speaker = element.FindPropertyRelative("SpeakerName");
            var text = element.FindPropertyRelative("Text");
            var audio = element.FindPropertyRelative("VoiceLine");
            var choices = element.FindPropertyRelative("Choices");
            var defaultNext = element.FindPropertyRelative("DefaultNextNodeID");
            var evt = element.FindPropertyRelative("EventTrigger");

            // --- HEADER (Collapsed View) ---
            string headerLabel = string.IsNullOrEmpty(nodeID.stringValue) ? "New Node" : $"{nodeID.stringValue} : {speaker.stringValue}";
            
            // Draw Foldout
            rect.y += 2;
            element.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.isExpanded, GUIContent.none);
            
            // Draw Label
            EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), headerLabel, EditorStyles.boldLabel);

            // --- BODY (Expanded View) ---
            if (element.isExpanded)
            {
                // Start drawing below the header
                float currentY = rect.y + EditorGUIUtility.singleLineHeight + 4;
                float spacing = 2f;

                // Helper to draw a property and advance Y
                void DrawProp(SerializedProperty prop)
                {
                    float height = EditorGUI.GetPropertyHeight(prop, true); // true = include children/arrays
                    EditorGUI.PropertyField(new Rect(rect.x, currentY, rect.width, height), prop, true);
                    currentY += height + spacing;
                }

                // Draw fields in specific order
                DrawProp(nodeID);
                DrawProp(speaker);
                DrawProp(text);
                DrawProp(audio);
                
                // Draw a separator line
                currentY += 5;
                EditorGUI.LabelField(new Rect(rect.x, currentY, rect.width, EditorGUIUtility.singleLineHeight), "Flow Control", EditorStyles.miniBoldLabel);
                currentY += EditorGUIUtility.singleLineHeight;

                DrawProp(choices);
                DrawProp(defaultNext);
                DrawProp(evt);
            }
        };

        // 3. Calculate Height
        _reorderableList.elementHeightCallback = (int index) =>
        {
            var element = _nodesProperty.GetArrayElementAtIndex(index);
            float baseHeight = EditorGUIUtility.singleLineHeight + 6; // Header height

            if (!element.isExpanded) return baseHeight;

            // Calculate total height of all properties if expanded
            float totalHeight = baseHeight + 4; // Initial padding
            
            // We must mirror the DrawProp logic here
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("NodeID")) + 2;
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("SpeakerName")) + 2;
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("Text"), true) + 2; // Text area is variable!
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("VoiceLine")) + 2;
            
            // Flow Control Header Padding
            totalHeight += EditorGUIUtility.singleLineHeight + 5; 

            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("Choices"), true) + 2; // List is variable!
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("DefaultNextNodeID")) + 2;
            totalHeight += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("EventTrigger")) + 2;

            return totalHeight + 10; // Bottom padding
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}