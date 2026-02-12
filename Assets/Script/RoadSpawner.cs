using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Orb
    public GameObject groundSegmentPrefab;   // GroundSegment prefab

    [Header("Settings")]
    public int initialSegments = 3;          // How many to spawn at start
    public float segmentWidth = 10f;         // Must match your prefab width
    public float spawnDistanceAhead = 15f;   // How far ahead of player to keep road
    public float despawnDistanceBehind = 20f;// How far behind player to remove road

    private List<Transform> activeSegments = new List<Transform>();

    private void Start()
    {
        // Spawn initial segments in a line
        float startX = 0f;
        for (int i = 0; i < initialSegments; i++)
        {
            Vector3 pos = new Vector3(startX + i * segmentWidth, 0f, 0f);
            GameObject seg = Instantiate(groundSegmentPrefab, pos, Quaternion.identity);
            activeSegments.Add(seg.transform);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Spawn new segment if the last one is too close to the player
        Transform lastSegment = activeSegments[activeSegments.Count - 1];
        float distanceToLast = lastSegment.position.x + segmentWidth - player.position.x;

        if (distanceToLast < spawnDistanceAhead)
        {
            SpawnNextSegment();
        }

        // Despawn segments that are far behind the player
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            Transform seg = activeSegments[i];
            if (player.position.x - seg.position.x > despawnDistanceBehind)
            {
                activeSegments.RemoveAt(i);
                Destroy(seg.gameObject);
            }
        }
    }

    private void SpawnNextSegment()
    {
        Transform lastSegment = activeSegments[activeSegments.Count - 1];
        Vector3 newPos = lastSegment.position + new Vector3(segmentWidth, 0f, 0f);
        GameObject seg = Instantiate(groundSegmentPrefab, newPos, Quaternion.identity);
        activeSegments.Add(seg.transform);
    }
}