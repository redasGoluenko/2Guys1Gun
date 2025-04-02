using UnityEngine;

public class Enemy1 : EnemyBase
{
    public float moveDistance = 5f;  
    private Vector3 startPosition;
    private bool movingRight = true;

    public override void Move()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= startPosition.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= startPosition.x)
            {
                movingRight = true;
            }
        }
    }

    protected override void Start()
    {
        base.Start(); 
        startPosition = transform.position; 
    }
    void Update()
    {
        Move();
    }
}
