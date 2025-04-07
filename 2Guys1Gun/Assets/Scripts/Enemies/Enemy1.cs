using UnityEngine; // Provides Unity core functionality like GameObjects, physics, etc.

public class Enemy1 : EnemyBase
{
    [Header("Movement Settings")]
    public float chaseDistance = 8f;         // Max distance to start chasing player
    public float stoppingDistance = 1f;      // Min distance to stop near player

    [Header("Collision Settings")]
    public float obstacleCheckDistance = 0.5f; // Distance to detect obstacles ahead
    public float shellRadius = 0.1f;          // Extra buffer for collision checks
    public LayerMask collisionLayer;          // Layers that count as solid objects

    private Rigidbody2D rb;                   // Physics component for movement
    private Collider2D col;                   // Collision box for detection
    private bool isFacingRight = true;        // Tracks if enemy faces right
    private Vector2 movementDirection;        // Direction toward player

    protected override void Start() // Unity: Called on object initialization
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>(); // Gets Rigidbody2D component
        col = GetComponent<Collider2D>(); // Gets Collider2D component
    }

    void Update() // Unity: Called every frame
    {
        if (targetPlayer == null) return;

        UpdateMovementDirection();
        HandleFacingDirection();
    }

    void FixedUpdate() // Unity: Called every physics step
    {
        Move();
        HandleVerticalCollision();
    }

    private void HandleVerticalCollision()
    {
        if (rb.velocity.y <= 0) // Check only when falling
        {
            float distance = Mathf.Abs(rb.velocity.y) * Time.fixedDeltaTime + col.bounds.extents.y + shellRadius; // Distance to check below

            RaycastHit2D hit = Physics2D.BoxCast( // Casts a box downward
                col.bounds.center,
                col.bounds.size * 0.9f,
                0f,
                Vector2.down,
                distance,
                collisionLayer);

            if (hit.collider != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); // Stop falling
                transform.position = new Vector2( // Snap to ground
                    transform.position.x,
                    hit.point.y + col.bounds.extents.y + shellRadius);
            }
        }
    }

    public override void Move()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position); // Distance to player

        if (distanceToPlayer > stoppingDistance && distanceToPlayer <= chaseDistance)
        {
            Vector2 moveVelocity = movementDirection * moveSpeed; // Set movement speed
            HandleHorizontalCollision(ref moveVelocity);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y); // Apply horizontal velocity
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop moving horizontally
        }
    }

    private void HandleHorizontalCollision(ref Vector2 moveVelocity)
    {
        float direction = Mathf.Sign(moveVelocity.x); // Get movement direction
        float distance = Mathf.Abs(moveVelocity.x) * Time.fixedDeltaTime + shellRadius; // Distance to check ahead

        RaycastHit2D hit = Physics2D.BoxCast( // Casts a box forward
            col.bounds.center,
            col.bounds.size * 0.9f,
            0f,
            Vector2.right * direction,
            distance,
            collisionLayer);

        if (hit.collider != null)
        {
            moveVelocity.x = 0; // Stop if hitting obstacle
        }
    }

    private void UpdateMovementDirection()
    {
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized; // Normalized direction to player
        movementDirection = new Vector2(directionToPlayer.x, 0); // Set horizontal direction
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
        isFacingRight = !isFacingRight; // Toggle facing direction
        Vector3 scale = transform.localScale; // Get current scale
        scale.x *= -1; // Flip horizontally
        transform.localScale = scale; // Apply new scale
    }
}