using UnityEngine;
using Pathfinding;

// used A* pathfinding project
// https://arongranberg.com/astar/

// require necessary components
[RequireComponent(typeof(Seeker), typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy1 : EnemyBase
{
    [Header("Pathfinding")]
    public float chaseDistance = 8f; // max distance to start chasing player
    public float nextWaypointDistance = 1f; // distance to switch to next waypoint
    public float updateRate = 0.5f; // path update interval

    [Header("Jumping")]
    public float jumpHeight = 3f; // jump height
    public float jumpCooldown = 0.5f; // cooldown between jumps
    public LayerMask groundLayer; // what is considered ground

    private Seeker seeker; // pathfinding component
    private Rigidbody2D rb; // physics body
    private Collider2D col; // collider for ground checks
    private Path path; // current path to player
    private int currentWaypoint = 0; // current path waypoint index
    private bool isFacingRight = true; // facing direction
    private float lastJumpTime; // last jump time
    private bool reachedEndOfPath; // reached final waypoint
    private float stuckCheckTimer = 0f; // time spent stuck
    private float stuckDurationThreshold = 1f; // time before considered stuck
    private float lastXPosition; // previous x position
    private bool wasStuck = false; // was stuck last check
    private Animator animator; // animation controller

    protected override void Start()
    {
        base.Start();
        seeker = GetComponent<Seeker>(); // get seeker
        rb = GetComponent<Rigidbody2D>(); // get rigidbody
        col = GetComponent<Collider2D>(); // get collider
        animator = GetComponent<Animator>(); // get animator

        col.isTrigger = false; // ensure collisions are on
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        InvokeRepeating(nameof(UpdatePath), 0f, updateRate); // keep path updated
        IgnorePlayerCollision(); // ignore player collision
        lastXPosition = rb.position.x; // store starting x pos
    }

    void IgnorePlayerCollision()
    {
        if (targetPlayer)
        {
            Collider2D playerCol = targetPlayer.GetComponent<Collider2D>();
            if (playerCol)
            {
                Physics2D.IgnoreCollision(col, playerCol, true); // turn off collision
            }
        }
    }

    void UpdatePath()
    {
        // update path if close enough and seeker ready
        if (targetPlayer && seeker.IsDone() &&
            Vector2.Distance(transform.position, targetPlayer.transform.position) <= chaseDistance)
        {
            seeker.StartPath(rb.position, targetPlayer.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p; // set new path
            currentWaypoint = 0; // reset progress
            reachedEndOfPath = false; // clear flag
        }
    }

    void Update()
    {
        if (path == null || targetPlayer == null) return;

        if (Vector2.Distance(transform.position, targetPlayer.transform.position) <= chaseDistance)
        {
            float xMoved = Mathf.Abs(rb.position.x - lastXPosition);

            if (xMoved < 0.01f) // barely moved
            {
                stuckCheckTimer += Time.deltaTime;

                if (stuckCheckTimer >= stuckDurationThreshold && !wasStuck)
                {
                    NudgeIfStuck(); // apply nudge
                    wasStuck = true;
                }
            }
            else
            {
                stuckCheckTimer = 0f; // reset stuck timer
                wasStuck = false;
                lastXPosition = rb.position.x; // update last x
            }
        }

        UpdateFacingDirection(); // flip if needed
        UpdateAnimationStates(); // update animator

        if (IsGrounded()) CheckForJump(); // try jumping
    }

    private void UpdateAnimationStates()
    {
        if (animator == null) return;
        float vertical = rb.velocity.y; // y speed
        float speed = Mathf.Abs(rb.velocity.x); // x speed
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Move(); // handle movement
    }

    public override void Move()
    {
        if (path == null || reachedEndOfPath) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true; // stop if done
            return;
        }

        Vector3 targetPosition = path.vectorPath[currentWaypoint];
        float distance = Vector2.Distance(rb.position, targetPosition);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++; // go to next point
            return;
        }

        Vector2 direction = ((Vector2)targetPosition - rb.position).normalized;
        bool isWallAhead = IsWallInDirection(Mathf.Sign(direction.x)); // wall check
        bool isBelowTarget = targetPosition.y > rb.position.y + 0.2f;

        if (IsGrounded() && isWallAhead && isBelowTarget)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // stop at wall
        }
        else
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y); // move forward
        }
    }

    private void UpdateFacingDirection()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 dir = path.vectorPath[currentWaypoint] - transform.position;

        if (dir.x > 0.1f && !isFacingRight) Flip(); // face right
        else if (dir.x < -0.1f && isFacingRight) Flip(); // face left
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // flip scale
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return col.IsTouchingLayers(groundLayer); // grounded check
    }

    private bool IsWallInDirection(float direction)
    {
        Vector2 origin = rb.position;
        Vector2 dir = new Vector2(direction, 0f);
        float distance = 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, groundLayer); // cast ray
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

        if (heightDifference > 0.5f && horizontalDistance > 0.2f &&
            rb.velocity.y <= 0.1f && !isWallAhead)
        {
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); // jump up
            lastJumpTime = Time.time; // reset jump timer
        }
    }

    private void NudgeIfStuck()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 nextWaypointPos = path.vectorPath[currentWaypoint];
        bool isBelowNextWaypoint = rb.position.y < nextWaypointPos.y;

        if (isBelowNextWaypoint)
        {
            float nudgeDirection = 0f;

            if (currentWaypoint + 1 < path.vectorPath.Count)
            {
                Vector2 nextNextWaypointPos = path.vectorPath[currentWaypoint + 1];
                nudgeDirection = Mathf.Sign(nextNextWaypointPos.x - nextWaypointPos.x); // look ahead
            }
            else
            {
                nudgeDirection = Mathf.Sign(nextWaypointPos.x - rb.position.x); // use current
            }

            if (!IsWallInDirection(nudgeDirection))
            {
                rb.velocity = new Vector2(nudgeDirection * moveSpeed * 0.5f, rb.velocity.y); // nudge forward
                return;
            }

            if (!IsWallInDirection(-nudgeDirection))
            {
                rb.velocity = new Vector2(-nudgeDirection * moveSpeed * 0.5f, rb.velocity.y); // nudge backward
                return;
            }
        }
        else
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            float nudgeDir = -Mathf.Sign(direction.x);

            if (!IsWallInDirection(nudgeDir))
            {
                rb.velocity = new Vector2(nudgeDir * 0.5f, rb.velocity.y); // nudge sideways
            }
        }
    }
}
