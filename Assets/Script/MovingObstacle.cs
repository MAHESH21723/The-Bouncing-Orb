using UnityEngine;

// Moves a red obstacle smoothly up and down.
public class MovingObstacle : MonoBehaviour
{
    public float amplitude = 1.4f;
    public float speed = 1.6f;

    private Vector3 startPosition;
    private float phase;

    private void Start()
    {
        startPosition = transform.position;
        phase = Random.value * Mathf.PI * 2f;
    }

    private void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * speed + phase) * amplitude;
    }
}
