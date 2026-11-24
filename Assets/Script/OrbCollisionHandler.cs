using UnityEngine;

// Scores a point for every bounce on the ground and resets the score when the orb hits a red obstacle.
public class OrbCollisionHandler : MonoBehaviour
{
    [Header("Tags")]
    public string groundTag = "Ground";
    public string resetObstacleTag = "ResetObstacle";

    private AudioSource audioSource;
    private float lastBounceTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        if (collision.collider.CompareTag(groundTag))
        {
            // Ignore tiny re-contacts while rolling so only real bounces count.
            if (Time.time - lastBounceTime < 0.12f || collision.relativeVelocity.magnitude < 1.2f) return;
            lastBounceTime = Time.time;
            if (audioSource != null) audioSource.Play();
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddBouncePoint();
        }
        else if (collision.collider.CompareTag(resetObstacleTag))
        {
            if (audioSource != null) audioSource.Play();
            if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
            if (GameManager.Instance != null) GameManager.Instance.OnHitObstacle(collision.collider.transform);
        }
    }
}
