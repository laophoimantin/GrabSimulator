using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OccasionalSpawnArea
{
    public string Name = "New Spawn Category";
    public OccasionalSoundType SoundToSpawn;

    [Tooltip("The list of physical boxes where this sound is allowed to spawn.")]
    public List<BoxCollider> SpawnVolumes = new List<BoxCollider>();

    [Header("Spawn Settings")]
    public float MinTime = 2f;
    public float MaxTime = 7f;
    [Range(1, 10)] public int MinAmount = 1;
    [Range(1, 10)] public int MaxAmount = 3;
    public float Volume = 0.8f;

    [HideInInspector] public Coroutine ActiveRoutine;
}

[RequireComponent(typeof(BoxCollider))]
public class OccasionalZone : MonoBehaviour
{
    [Header("Optimization (Player Culling Box)")]
    [Tooltip("The width/length of the active area around the player. Red boxes outside this are ignored!")]
    public float activeAreaSize = 40f;

    [Header("List of Red Boxes (Spawn Areas)")]
    public List<OccasionalSpawnArea> spawnAreas = new List<OccasionalSpawnArea>();

    private Transform playerTransform;
    private BoxCollider mainZoneCollider;

    private void Awake()
    {
        mainZoneCollider = GetComponent<BoxCollider>();
        mainZoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;

            foreach (var area in spawnAreas)
            {
                if (area.SpawnVolumes.Count > 0 && area.ActiveRoutine == null)
                {
                    area.ActiveRoutine = StartCoroutine(SpawnRoutine(area));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;

            foreach (var area in spawnAreas)
            {
                if (area.ActiveRoutine != null)
                {
                    StopCoroutine(area.ActiveRoutine);
                    area.ActiveRoutine = null;
                }
            }
        }
    }

    private IEnumerator SpawnRoutine(OccasionalSpawnArea area)
    {
        while (true)
        {
            float waitTime = Random.Range(area.MinTime, area.MaxTime);
            yield return new WaitForSeconds(waitTime);

            if (playerTransform == null || area.SpawnVolumes.Count == 0) break;

            Bounds playerCullingBounds = new Bounds(
                new Vector3(playerTransform.position.x, mainZoneCollider.bounds.center.y, playerTransform.position.z),
                new Vector3(activeAreaSize, mainZoneCollider.bounds.size.y, activeAreaSize)
            );

            List<BoxCollider> availableBoxes = new List<BoxCollider>();
            foreach (var box in area.SpawnVolumes)
            {
                if (box != null && playerCullingBounds.Intersects(box.bounds))
                {
                    availableBoxes.Add(box);
                }
            }

            if (availableBoxes.Count == 0) continue;
            
            int randomAmountOfBox = Random.Range(1, availableBoxes.Count + 1);

            for (int i = 0; i < randomAmountOfBox; i++)
            {
                BoxCollider selectedBox = availableBoxes[Random.Range(0, availableBoxes.Count)];

                int spawnAmount = Random.Range(area.MinAmount, area.MaxAmount + 1);

                for (int j = 0; j < spawnAmount; j++)
                {
                    if (selectedBox == null) break;

                    Vector3 randomLocalPoint = new Vector3(
                        Random.Range(-selectedBox.size.x / 2f, selectedBox.size.x / 2f),
                        Random.Range(-selectedBox.size.y / 2f, selectedBox.size.y / 2f),
                        Random.Range(-selectedBox.size.z / 2f, selectedBox.size.z / 2f)
                    ) + selectedBox.center;

                    Vector3 worldSpawnPos = selectedBox.transform.TransformPoint(randomLocalPoint);
                    SoundManager.Instance.PlaySound3D(area.SoundToSpawn, worldSpawnPos, area.Volume);

                    yield return null;
                }

                availableBoxes.Remove(selectedBox);
            }
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider masterBox = GetComponent<BoxCollider>();
        if (masterBox == null) return;

        // Draw the Master Trigger Zone
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(masterBox.center, masterBox.size);

        // Draw the Inner Spawn Boxes accurately
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        foreach (var area in spawnAreas)
        {
            foreach (var box in area.SpawnVolumes)
            {
                if (box != null)
                {
                    Gizmos.matrix = box.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(box.center, box.size);
                }
            }
        }

        // Draw the Player's Culling Box 
        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.matrix = Matrix4x4.identity; // Reset matrix to world space for the player
            Gizmos.color = new Color(0f, 1f, 1f, 0.8f); // Bright Cyan

            Vector3 center = new Vector3(playerTransform.position.x, masterBox.bounds.center.y, playerTransform.position.z);
            Vector3 size = new Vector3(activeAreaSize, masterBox.bounds.size.y, activeAreaSize);

            Gizmos.DrawWireCube(center, size);
        }
    }
}