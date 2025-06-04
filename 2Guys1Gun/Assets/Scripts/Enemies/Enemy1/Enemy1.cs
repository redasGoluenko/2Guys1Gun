using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy1 : EnemyBase
{
    [Header("Pathfinding")]
    public float chaseDistance = 8f;
    public float nextWaypointDistance = 1f;
    public float updateRate = 0.5f;

    [Header("Jumping")]
    public float jumpHeight = 3f;
    public float jumpCooldown = 0.5f;
    public LayerMask groundLayer;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Collider2D col;
    private Path path;
    private int currentWaypoint = 0;
    private bool isFacingRight = true;
    private float lastJumpTime;
    private bool reachedEndOfPath;

    // Stuck detection fields
    private float stuckCheckTimer = 0f;
    private float stuckDurationThreshold = 1f;
    private float lastXPosition;
    private bool wasStuck = false;

    protected override void Start()
    {
        base.Start();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        col.isTrigger = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        InvokeRepeating(nameof(UpdatePath), 0f, updateRate);
        IgnorePlayerCollision();

        lastXPosition = rb.position.x;
    }

    void IgnorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol)
            {
                Physics2D.IgnoreCollision(col, playerCol, true);
            }
        }
    }

    void UpdatePath()
    {
        if (targetPlayer && seeker.IsDone() && Vector2.Distance(transform.position, targetPlayer.position) <= chaseDistance)
        {
            seeker.StartPath(rb.position, targetPlayer.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            reachedEndOfPath = false;
        }
    }

    void Update()
    {
        if (path == null || targetPlayer == null) return;

        // Only check for stuck if within chase range
        if (Vector2.Distance(transform.position, targetPlayer.position) <= chaseDistance)
        {
            float xMoved = Mathf.Abs(rb.position.x - lastXPosition);

            if (xMoved < 0.01f)
            {
                stuckCheckTimer += Time.deltaTime;
                if (stuckCheckTimer >= stuckDurationThreshold && !wasStuck)
                {
                    NudgeIfStuck();
                    wasStuck = true;
                }
            }
            else
            {
                stuckCheckTimer = 0f;
                wasStuck = false;
                lastXPosition = rb.position.x;
            }
        }

        UpdateFacingDirection();

        if (IsGrounded())
        {
            CheckForJump();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        if (path == null || reachedEndOfPath) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        Vector3 targetPosition = path.vectorPath[currentWaypoint];
        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }

        Vector2 direction = ((Vector2)targetPosition - rb.position).normalized;
        bool isWallAhead = IsWallInDirection(Mathf.Sign(direction.x));
        bool isBelowTarget = targetPosition.y > rb.position.y + 0.2f;

        // ✅ Handle case where enemy is directly above player and might be stuck
        if (targetPlayer != null)
        {
            float xDiff = Mathf.Abs(targetPlayer.position.x - rb.position.x);
            float yDiff = rb.position.y - targetPlayer.position.y;

            if (xDiff < 0.5f && yDiff > 0.5f) // close X, player clearly below
            {
                // move slightly to the right if no wall
                if (!IsWallInDirection(1f))
                {
                    rb.velocity = new Vector2(1f, rb.velocity.y); // slight nudge right
                    return;
                }
            }
        }

        // Standard horizontal move unless blocked or falling
        if (IsGrounded() && isWallAhead && isBelowTarget)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // let it fall
        }
        else
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
    }

    private void UpdateFacingDirection()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 dir = path.vectorPath[currentWaypoint] - transform.position;
        if (dir.x > 0.1f && !isFacingRight) Flip();
        else if (dir.x < -0.1f && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return col.IsTouchingLayers(groundLayer);
    }

    private bool IsWallInDirection(float direction)
    {
        Vector2 origin = rb.position;
        Vector2 dir = new Vector2(direction, 0f);
        float distance = 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, groundLayer);
        return hit.collider != null;
    }

    private void CheckForJump()
    {
        if (!IsGrounded()) return;
        if (Time.time < lastJumpTime + jumpCooldown) return;
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 currentPos = rb.position;
        Vector2 nextWaypoint = path.vectorPath[currentWaypoint];
        float heightDifference = nextWaypoint.y - currentPos.y;
        float horizontalDistance = Mathf.Abs(nextWaypoint.x - currentPos.x);

        bool isWallAhead = IsWallInDirection(Mathf.Sign(nextWaypoint.x - currentPos.x));

        if (heightDifference > 0.5f && horizontalDistance > 0.2f && rb.velocity.y <= 0.1f && !isWallAhead)
        {
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            lastJumpTime = Time.time;
        }
    }

    private void NudgeIfStuck()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        // Get the next waypoint's position
        Vector2 nextWaypointPos = path.vectorPath[currentWaypoint];

        // Check if we're below the next waypoint
        bool isBelowNextWaypoint = rb.position.y < nextWaypointPos.y;

        if (isBelowNextWaypoint)
        {
            // Determine which direction to nudge based on the path direction
            float nudgeDirection = 0f;

            // Check if we can get the waypoint after next for better direction
            if (currentWaypoint + 1 < path.vectorPath.Count)
            {
                Vector2 nextNextWaypointPos = path.vectorPath[currentWaypoint + 1];
                nudgeDirection = Mathf.Sign(nextNextWaypointPos.x - nextWaypointPos.x);
            }
            else
            {
                // If no next waypoint, use the direction to the current waypoint
                nudgeDirection = Mathf.Sign(nextWaypointPos.x - rb.position.x);
            }

            // Try nudging in the path direction first
            if (!IsWallInDirection(nudgeDirection))
            {
                rb.velocity = new Vector2(nudgeDirection * moveSpeed * 0.5f, rb.velocity.y);
                return;
            }

            // If that direction is blocked, try the opposite direction
            if (!IsWallInDirection(-nudgeDirection))
            {
                rb.velocity = new Vector2(-nudgeDirection * moveSpeed * 0.5f, rb.velocity.y);
                return;
            }
        }
        else
        {
            // Original behavior if not below waypoint
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            float nudgeAmount = 0.5f;
            float nudgeDir = -Mathf.Sign(direction.x); // Move opposite to current target direction

            // Only nudge if no wall on the new side
            if (!IsWallInDirection(nudgeDir))
            {
                rb.velocity = new Vector2(nudgeDir * nudgeAmount, rb.velocity.y);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rb.position, rb.position + new Vector2(0.3f * Mathf.Sign(transform.localScale.x), 0));
    }
}