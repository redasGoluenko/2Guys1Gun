using UnityEngine;

public class MoveBackAndForth : MonoBehaviour
{
    public float moveDistance = 5f; // Distance to move to the right
    public float moveSpeed = 2f; // Speed of movement
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= startPosition.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position -= Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x <= startPosition.x)
            {
                movingRight = true;
            }
        }
    }
}
