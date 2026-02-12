using System.Collections.Generic;
using UnityEngine;

public class RandomObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // Orb
    public GameObject obstaclePrefab;     // ResetObstacle prefab

    [Header("Spawn Settings")]
    public float minXSpacing = 10f;       // Min distance between obstacles
    public float maxXSpacing = 20f;       // Max distance between obstacles
    public float minY = 3f;               // Random Y min
    public float maxY = 5f;               // Random Y max
    public float spawnAheadDistance = 30f;
    public float despawnBehindDistance = 20f;

    private float nextSpawnX;
    private List<Transform> activeObstacles = new List<Transform>();

    private void Start()
    {
        // First obstacle starts some distance ahead of the player
        if (player != null)
            nextSpawnX = player.position.x + minXSpacing;
        else
            nextSpawnX = 5f;
    }

    private void Update()
    {
        if (player == null) return;

        // Spawn obstacles ahead of the player
        while (nextSpawnX < player.position.x + spawnAheadDistance)
        {
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(nextSpawnX, randomY, 0f);
            GameObject obs = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            activeObstacles.Add(obs.transform);

            // Decide next X using random spacing
            float randomSpacing = Random.Range(minXSpacing, maxXSpacing);
            nextSpawnX += randomSpacing;
        }

        // Despawn obstacles that are far behind
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            Transform obs = activeObstacles[i];
            if (player.position.x - obs.position.x > despawnBehindDistance)
            {
                activeObstacles.RemoveAt(i);
                Destroy(obs.gameObject);
            }
        }
    }
}