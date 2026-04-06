using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OccasionalZone))]
public class OccasionalZoneEditor : Editor
{
    private static Mesh cubeMesh;

    private static Mesh GetCubeMesh()
    {
        if (cubeMesh == null)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = temp.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(temp);
        }
        return cubeMesh;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OccasionalZone zone = (OccasionalZone)target;

        GUILayout.Space(20);
        GUILayout.Label("Level Designer Automation Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Brand New Sound Category", GUILayout.Height(30)))
        {
            Undo.RecordObject(zone, "Add Sound Category");

            OccasionalSpawnArea newArea = new OccasionalSpawnArea();
            newArea.Name = "Category " + (zone.spawnAreas.Count + 1);
            zone.spawnAreas.Add(newArea);

            EditorUtility.SetDirty(zone);
        }

        GUILayout.Space(10);

        for (int i = 0; i < zone.spawnAreas.Count; i++)
        {
            OccasionalSpawnArea currentArea = zone.spawnAreas[i];

            if (GUILayout.Button($"Add Box GameObject to '{currentArea.Name}'"))
            {
                GameObject newBox = new GameObject($"{currentArea.Name}_Box_{currentArea.SpawnVolumes.Count + 1}");

                newBox.transform.SetParent(zone.transform);
                newBox.transform.localPosition = Vector3.zero;

                BoxCollider col = newBox.AddComponent<BoxCollider>();
                col.isTrigger = true;

                MeshFilter meshFilter = newBox.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = GetCubeMesh();

                Undo.RecordObject(zone, "Add Box to Category");
                currentArea.SpawnVolumes.Add(col);

                Selection.activeGameObject = newBox;
                EditorUtility.SetDirty(zone);

                GUIUtility.ExitGUI();
            }
        }
    }
}