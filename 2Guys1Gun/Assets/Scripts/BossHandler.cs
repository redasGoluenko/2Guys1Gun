using UnityEngine;

public class BossHandler2D : MonoBehaviour
{
    [Header("Player References")]
    public WeaponTransfer playerA;
    public WeaponTransfer playerB;

    [Header("Movement Settings")]
    public float smoothSpeed = 5f;
    public float descentSpeed = 0.5f;
    public float retreatSpeed = 2f;
    public float maxDescentY = 2f;

    [Header("Damage Settings")]
    public float damageWindowDuration = 1.5f;

    [Header("Visuals")]
    public Color vulnerableColor = Color.red;

    [Header("Spawning Settings")]
    public GameObject spawnPrefab;    // Single prefab to spawn
    public float spawnInterval = 3f;  // Seconds between spawns
    public Vector2 spawnOffset = Vector2.zero; // Offset from boss position to spawn
    public float projectileSpeed = 10f; // Speed of projectile
    public float projectileLifetime = 5f; // Time before projectile destroys itself

    [Header("Platform References")]
    public Transform leftPlatformsParent;
    public Transform rightPlatformsParent;

    [Header("Platform Movement")]
    public float platformRaiseHeight = 3f;
    public float platformRaiseSpeed = 2f;

    private Transform currentTarget;
    private WeaponTransfer currentHolder;

    private bool isRetreating = false;
    private float damageWindowTimer = 0f;
    private float originalY;

    private SpriteRenderer bossRenderer;
    private Color originalColor;

    private float spawnTimer = 0f;

    private Vector3 leftPlatformsOriginalPos;
    private Vector3 rightPlatformsOriginalPos;

    [Header("Platform Retract Delay")]
    public float retractDelay = 1.5f;

    private float leftRetractTimer = 0f;
    private float rightRetractTimer = 0f;

    private bool leftIsRaised = false;
    private bool rightIsRaised = false;

    private bool leftShouldRetract = false;
    private bool rightShouldRetract = false;

    // Added boolean for target switching
    private bool isSwitchingTarget = false;

    void Start()
    {
        originalY = transform.position.y;

        bossRenderer = GetComponent<SpriteRenderer>();
        if (bossRenderer != null)
        {
            originalColor = bossRenderer.color;
        }

        spawnTimer = spawnInterval; // Start spawn timer ready

        if (leftPlatformsParent != null)
            leftPlatformsOriginalPos = leftPlatformsParent.position;
        if (rightPlatformsParent != null)
            rightPlatformsOriginalPos = rightPlatformsParent.position;
    }

    void Update()
    {
        UpdateGunHolder();

        float speedModifier = isSwitchingTarget ? 0.5f : 1f;

        Vector3 pos = transform.position;

        if (isRetreating)
        {
            damageWindowTimer -= Time.deltaTime;

            if (pos.y < originalY)
            {
                pos.y += retreatSpeed * speedModifier * Time.deltaTime;
                if (pos.y > originalY)
                {
                    pos.y = originalY; // Snap to originalY
                }
            }

            transform.position = pos;

            if (damageWindowTimer <= 0f && pos.y >= originalY)
            {
                isRetreating = false;
            }
        }
        else
        {
            if (currentTarget != null && pos.y > maxDescentY)
            {
                pos.y -= descentSpeed * speedModifier * Time.deltaTime;
                transform.position = pos;
            }
        }

        // Follow target on X axis
        if (currentTarget != null)
        {
            pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, currentTarget.position.x, smoothSpeed * speedModifier * Time.deltaTime);
            transform.position = pos;
        }

        UpdateColor();

        HandleSpawning();

        HandlePlatformMovement();
    }


    void UpdateGunHolder()
    {
        WeaponTransfer newHolder = null;

        if (playerA != null && playerA.hasBall)
            newHolder = playerA;
        else if (playerB != null && playerB.hasBall)
            newHolder = playerB;

        if (newHolder != currentHolder)
        {
            isSwitchingTarget = true;  // <-- target is switching
            currentHolder = newHolder;
            currentTarget = currentHolder != null ? currentHolder.transform : null;

            if (currentHolder != null)
            {
                TriggerRetreat();
            }
        }
        else
        {
            isSwitchingTarget = false; // no switch if same target
        }
    }

    void TriggerRetreat()
    {
        isRetreating = true;
        damageWindowTimer = damageWindowDuration;
        Debug.Log("Boss retreating — weak point exposed!");
    }

    public bool CanTakeDamage()
    {
        return isRetreating && damageWindowTimer > 0f;
    }

    void UpdateColor()
    {
        if (bossRenderer == null) return;

        if (CanTakeDamage())
        {
            bossRenderer.color = vulnerableColor;
        }
        else
        {
            bossRenderer.color = originalColor;
        }
    }

    void HandleSpawning()
    {
        if (spawnPrefab == null) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnObject();
            spawnTimer = spawnInterval; // Reset timer
        }
    }

    void SpawnObject()
    {
        if (currentTarget == null)
            return;

        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        GameObject projectile = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb2d = projectile.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            Vector2 direction = (currentTarget.position - (Vector3)spawnPos).normalized;
            rb2d.velocity = Vector2.zero; // reset velocity first
            rb2d.AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        }

        Destroy(projectile, projectileLifetime);  // Destroy after lifetime seconds

        Debug.Log($"Shot {spawnPrefab.name} toward {currentTarget.name}");
    }

    void HandlePlatformMovement()
    {
        bool shouldRaiseLeft = (currentTarget == playerB?.transform);
        bool shouldRaiseRight = (currentTarget == playerA?.transform);

        // LEFT SIDE
        if (shouldRaiseLeft)
        {
            MovePlatforms(leftPlatformsParent, leftPlatformsOriginalPos + Vector3.up * platformRaiseHeight);
            leftRetractTimer = retractDelay;
            leftIsRaised = true;
            leftShouldRetract = false;
        }
        else if (leftIsRaised)
        {
            if (!leftShouldRetract)
            {
                leftRetractTimer -= Time.deltaTime;
                if (leftRetractTimer <= 0f)
                {
                    leftShouldRetract = true;
                }
            }
            else
            {
                MovePlatforms(leftPlatformsParent, leftPlatformsOriginalPos);
                if (Vector3.Distance(leftPlatformsParent.position, leftPlatformsOriginalPos) < 0.05f)
                {
                    leftIsRaised = false;
                    leftShouldRetract = false;
                }
            }
        }

        // RIGHT SIDE
        if (shouldRaiseRight)
        {
            MovePlatforms(rightPlatformsParent, rightPlatformsOriginalPos + Vector3.up * platformRaiseHeight);
            rightRetractTimer = retractDelay;
            rightIsRaised = true;
            rightShouldRetract = false;
        }
        else if (rightIsRaised)
        {
            if (!rightShouldRetract)
            {
                rightRetractTimer -= Time.deltaTime;
                if (rightRetractTimer <= 0f)
                {
                    rightShouldRetract = true;
                }
            }
            else
            {
                MovePlatforms(rightPlatformsParent, rightPlatformsOriginalPos);
                if (Vector3.Distance(rightPlatformsParent.position, rightPlatformsOriginalPos) < 0.05f)
                {
                    rightIsRaised = false;
                    rightShouldRetract = false;
                }
            }
        }
    }

    void MovePlatforms(Transform platformParent, Vector3 targetPos)
    {
        if (platformParent == null) return;

        platformParent.position = Vector3.Lerp(platformParent.position, targetPos, platformRaiseSpeed * Time.deltaTime);
    }
}
