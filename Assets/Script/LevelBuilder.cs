using UnityEngine;

// Builds a level from LevelData: ground with pits, red obstacles, the finish flag and some scenery.
// (It replaces the endless RoadSpawner and RandomObstacleSpawner from the first version of the game.)
public class LevelBuilder : MonoBehaviour
{
    public GameObject groundSegmentPrefab;
    public GameObject obstaclePrefab;
    public Sprite circleSprite;

    public float FinishX { get; private set; }
    public Vector2 StartPosition => new Vector2(0f, 2.5f);

    private Transform root;
    private static readonly Color ObstacleRed = new Color(0.9f, 0.22f, 0.2f);

    public void Build(int levelIndex)
    {
        Clear();
        LevelData data = LevelData.All[levelIndex];
        // Same layout every time you replay a level.
        Random.State oldState = Random.state;
        Random.InitState(1000 + levelIndex * 37);

        root = new GameObject("Level " + (levelIndex + 1)).transform;
        FinishX = data.length;
        Camera.main.backgroundColor = data.sky;

        BuildScenery(data);
        BuildGround(data);
        BuildObstacles(data);
        BuildFinishFlag(data);

        Random.state = oldState;
    }

    public void Clear()
    {
        if (root != null) Destroy(root.gameObject);
        root = null;
    }

    private void BuildGround(LevelData data)
    {
        float x = -12f;
        bool first = true;
        while (x < data.length + 25f)
        {
            float width = first ? 24f : Random.Range(8f, 14f);
            first = false;
            AddGround(x, width, data.ground);
            x += width;

            // A pit (gap) after this piece, but never right before the finish.
            if (x > 10f && x < data.length - 12f && Random.value < data.pitChance)
            {
                x += Random.Range(2.4f, 3.2f);
            }
        }
    }

    private void AddGround(float left, float width, Color color)
    {
        var ground = Instantiate(groundSegmentPrefab, new Vector3(left + width / 2f, 0f, 0f), Quaternion.identity, root);
        ground.transform.localScale = new Vector3(width, 1f, 1f);
        ground.GetComponent<SpriteRenderer>().color = color;

        // Darker soil under the grass, so the ground doesn't look like it is floating.
        var soil = MakeSprite("Soil", ground.GetComponent<SpriteRenderer>().sprite, new Vector3(left + width / 2f, -4.5f, 0f),
            new Vector3(width, 8f, 1f), Color.Lerp(color, new Color(0.25f, 0.17f, 0.1f), 0.65f), -1);
        soil.transform.SetParent(root);
    }

    private void BuildObstacles(LevelData data)
    {
        float x = 12f;
        while (x < data.length - 6f)
        {
            float y = Random.Range(data.minObstacleY, data.maxObstacleY);
            var obstacle = Instantiate(obstaclePrefab, new Vector3(x, y, 0f), Quaternion.identity, root);
            obstacle.GetComponent<SpriteRenderer>().color = ObstacleRed;
            obstacle.GetComponent<SpriteRenderer>().sortingOrder = 2;

            if (Random.value < data.movingChance)
            {
                var body = obstacle.AddComponent<Rigidbody2D>();
                body.bodyType = RigidbodyType2D.Kinematic;
                obstacle.AddComponent<MovingObstacle>();
                obstacle.GetComponent<SpriteRenderer>().color = new Color(1f, 0.42f, 0.16f);
            }
            x += Random.Range(data.minSpacing, data.maxSpacing);
        }
    }

    private void BuildFinishFlag(LevelData data)
    {
        Sprite square = obstaclePrefab.GetComponent<SpriteRenderer>().sprite;
        float x = data.length;
        MakeSprite("Flag Pole", square, new Vector3(x, 3f, 0f), new Vector3(0.22f, 5f, 1f), Color.white, 1).transform.SetParent(root);
        // A small checkered flag.
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Color c = (row + col) % 2 == 0 ? new Color(0.1f, 0.1f, 0.1f) : Color.white;
                MakeSprite("Flag", square, new Vector3(x + 0.31f + col * 0.4f, 5.1f - row * 0.4f, 0f), new Vector3(0.4f, 0.4f, 1f), c, 1)
                    .transform.SetParent(root);
            }
        }
        MakeSprite("Finish Line", square, new Vector3(x, 0.52f, 0f), new Vector3(0.5f, 0.06f, 1f), Color.white, 1).transform.SetParent(root);
    }

    private void BuildScenery(LevelData data)
    {
        Color hill = Color.Lerp(data.ground, data.sky, 0.45f);
        Color farHill = Color.Lerp(data.ground, data.sky, 0.7f);
        for (float x = -20f; x < data.length + 40f; x += Random.Range(10f, 18f))
        {
            float size = Random.Range(10f, 16f);
            MakeSprite("Far Hill", circleSprite, new Vector3(x + 6f, -size * 0.2f, 1f), Vector3.one * size * 1.3f, farHill, -20).transform.SetParent(root);
            MakeSprite("Hill", circleSprite, new Vector3(x, -size * 0.32f, 1f), Vector3.one * size, hill, -15).transform.SetParent(root);
        }
        for (float x = -10f; x < data.length + 40f; x += Random.Range(12f, 22f))
        {
            float y = Random.Range(8.5f, 11f);
            Color cloud = new Color(1f, 1f, 1f, 0.75f);
            MakeSprite("Cloud", circleSprite, new Vector3(x, y, 1f), new Vector3(3.2f, 1.6f, 1f), cloud, -10).transform.SetParent(root);
            MakeSprite("Cloud", circleSprite, new Vector3(x + 1.3f, y + 0.4f, 1f), new Vector3(2.4f, 1.8f, 1f), cloud, -10).transform.SetParent(root);
        }
    }

    private static GameObject MakeSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, Color color, int order)
    {
        var go = new GameObject(name, typeof(SpriteRenderer));
        go.transform.position = position;
        go.transform.localScale = scale;
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = order;
        return go;
    }
}
