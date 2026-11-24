using UnityEngine;

// Follows the orb sideways (and a little in height) without rotating with it.
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float lookAhead = 3f;
    public float baseHeight = 4.2f;
    public float smooth = 6f;

    private void LateUpdate()
    {
        if (target == null) return;
        float y = baseHeight + Mathf.Max(0f, target.position.y - 6f) * 0.5f;
        Vector3 wanted = new Vector3(target.position.x + lookAhead, y, -10f);
        transform.position = Vector3.Lerp(transform.position, wanted, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }

    public void Snap()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x + lookAhead, baseHeight, -10f);
    }
}
