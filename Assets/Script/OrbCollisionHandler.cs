using UnityEngine;
public class OrbCollisionHandler : MonoBehaviour
{
    [Header("Tags")]
    public string groundTag = "Ground";
    public string resetObstacleTag = "ResetObstacle";
    private AudioSource audioSource;
    private bool lastCollisionWasGround = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }       
        if (collision.collider.CompareTag(groundTag))
        {
         
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddBouncePoint();
            }

            lastCollisionWasGround = true;
        }
        else if (collision.collider.CompareTag(resetObstacleTag))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetScore();
            }

            lastCollisionWasGround = false;
        }
        else
        {
            lastCollisionWasGround = false;
        }
    }
}
