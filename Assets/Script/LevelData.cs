using UnityEngine;

// The settings for each level. Later levels are longer, with more obstacles, pits and moving blocks.
[System.Serializable]
public struct LevelData
{
    public float length;          // distance from the start to the finish flag
    public float minSpacing;      // gap between red obstacles
    public float maxSpacing;
    public float minObstacleY;    // height range of the red obstacles
    public float maxObstacleY;
    public float pitChance;       // chance of a pit after each piece of ground
    public float movingChance;    // chance that an obstacle moves up and down
    public int targetBounces;     // bounces needed for 2 stars (3 stars = 1.5x)
    public Color sky;
    public Color ground;

    public LevelData(float length, float minSpacing, float maxSpacing, float minY, float maxY,
                     float pitChance, float movingChance, int target, string sky, string ground)
    {
        this.length = length;
        this.minSpacing = minSpacing;
        this.maxSpacing = maxSpacing;
        minObstacleY = minY;
        maxObstacleY = maxY;
        this.pitChance = pitChance;
        this.movingChance = movingChance;
        targetBounces = target;
        ColorUtility.TryParseHtmlString(sky, out this.sky);
        ColorUtility.TryParseHtmlString(ground, out this.ground);
    }

    public static readonly LevelData[] All =
    {
        //            length spacing   obstacle y  pits  moving target  sky        ground
        new LevelData( 60f, 12f, 18f, 3.0f, 4.5f, 0.00f, 0.00f, 7, "#7ec8f0", "#5aa83c"),
        new LevelData( 75f, 10f, 16f, 2.6f, 4.5f, 0.00f, 0.00f, 9, "#82c4ec", "#58a03a"),
        new LevelData( 85f, 10f, 15f, 2.5f, 4.5f, 0.15f, 0.00f, 11, "#f2b880", "#8a9a3c"),
        new LevelData( 95f,  9f, 14f, 2.2f, 4.5f, 0.20f, 0.00f, 12, "#f4a46a", "#7e8c36"),
        new LevelData(105f,  9f, 13f, 2.2f, 4.5f, 0.20f, 0.20f, 14, "#d98ab4", "#5e8a4a"),
        new LevelData(115f,  8f, 12f, 2.0f, 4.2f, 0.25f, 0.30f, 15, "#b98ad9", "#4f7d55"),
        new LevelData(125f,  8f, 12f, 1.9f, 4.2f, 0.30f, 0.35f, 17, "#7e8fe0", "#476e5c"),
        new LevelData(135f,  7f, 11f, 1.9f, 4.0f, 0.30f, 0.40f, 18, "#5d73c9", "#3f6358"),
        new LevelData(145f,  7f, 10f, 1.8f, 4.0f, 0.35f, 0.45f, 20, "#3f4f9e", "#3a5a52"),
        new LevelData(160f,  6f, 10f, 1.8f, 4.0f, 0.35f, 0.50f, 22, "#26305e", "#34504c"),
    };
}
