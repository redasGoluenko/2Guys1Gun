using UnityEngine;

//Enemy1 script to handle the enemy's movement and collision detection
public class Enemy1 : EnemyBase
{
    [Header("Movement Settings")]
    public float chaseDistance = 8f; //distance to chase the player
    public float stoppingDistance = 1f; //distance to stop chasing the player

    [Header("Collision Settings")]
    public float obstacleCheckDistance = 0.5f; //distance to check for obstacles
    public float shellRadius = 0.1f; //to avoid getting stuck in small gaps   
    public LayerMask collisionLayer;          

    private Rigidbody2D rb;             
    private Collider2D col;                   
    private bool isFacingRight = true;        
    private Vector2 movementDirection;       

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (targetPlayer == null) return;

        UpdateMovementDirection();
        HandleFacingDirection();
    }

    void FixedUpdate()
    {
        Move();
        HandleVerticalCollision();
    }

    //For vertical collision detection
    private void HandleVerticalCollision()
    {
        if (rb.velocity.y <= 0)
        {
            float distance = Mathf.Abs(rb.velocity.y) * Time.fixedDeltaTime + col.bounds.extents.y + shellRadius;

            RaycastHit2D hit = Physics2D.BoxCast(
                col.bounds.center,
                col.bounds.size * 0.9f,
                0f,
                Vector2.down,
                distance,
                collisionLayer);

            if (hit.collider != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                transform.position = new Vector2(
                    transform.position.x,
                    hit.point.y + col.bounds.extents.y + shellRadius);
            }
        }
    }

    //For horizontal movement
    public override void Move()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer > stoppingDistance && distanceToPlayer <= chaseDistance)
        {
            Vector2 moveVelocity = movementDirection * moveSpeed;
            HandleHorizontalCollision(ref moveVelocity);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    //For horizontal collision detection
    private void HandleHorizontalCollision(ref Vector2 moveVelocity)
    {
        float direction = Mathf.Sign(moveVelocity.x);
        float distance = Mathf.Abs(moveVelocity.x) * Time.fixedDeltaTime + shellRadius;

        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size * 0.9f,
            0f,
            Vector2.right * direction,
            distance,
            collisionLayer);

        if (hit.collider != null)
        {
            moveVelocity.x = 0;
        }
    }
    
    private void UpdateMovementDirection()
    {
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        movementDirection = new Vector2(directionToPlayer.x, 0);
    }

    private void HandleFacingDirection()
    {
        if (movementDirection.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (movementDirection.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}