using UnityEngine;

public class CameraLimiter : MonoBehaviour
{
    public Transform target;
    public Vector2 minBounds;
    public Vector2 maxBounds; 

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);
        newPosition.z = transform.position.z; 

        transform.position = newPosition;
    }
}
