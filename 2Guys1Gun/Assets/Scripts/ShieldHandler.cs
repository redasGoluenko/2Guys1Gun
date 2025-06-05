using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    public bool isLeftPlayer = true;
    [Tooltip("Key to activate the shield")]
    public KeyCode activationKey;

    [Tooltip("Speed of the scaling animation")]
    public float scaleSpeed = 5f;

    [Tooltip("Duration the shield stays active (seconds)")]
    public float activeDuration = 3f;

    [Tooltip("Cooldown before the shield can be used again (seconds)")]
    public float cooldownDuration = 10f;

    [Tooltip("Reference to the player GameObject")]
    public GameObject player;

    private Vector3 originalScale;
    private Vector3 hiddenScale = Vector3.one * 0.01f;

    private bool isScalingUp = false;
    private bool isScalingDown = false;
    private bool isOnCooldown = false;

    private float shieldTimer = 0f;
    private float cooldownTimer = 0f;
    public bool isShieldActive = false;

    private Renderer shieldRenderer;

    void Start()
    {
        activationKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Down");
        originalScale = transform.localScale;
        transform.localScale = hiddenScale;

        shieldRenderer = GetComponent<Renderer>();
        if (shieldRenderer != null)
            shieldRenderer.enabled = false;
    }

    void Update()
    {
        // Cooldown logic
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                cooldownTimer = 0f;
                isOnCooldown = false;
            }
        }

        // Activate shield
        if (Input.GetKeyDown(activationKey) && !isOnCooldown && !isScalingUp && !isScalingDown)
        {
            if (!PlayerHasWeapon())
            {
                if (shieldRenderer != null)
                    shieldRenderer.enabled = true;

                isScalingUp = true;
                isOnCooldown = true;
                shieldTimer = 0f;
                isShieldActive = true;
            }
            else
            {
                Debug.Log("Shield blocked: Player is holding a weapon.");
            }
        }

        // Scale up animation
        if (isScalingUp)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed);
            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                transform.localScale = originalScale;
                isScalingUp = false;
            }
        }

        // Shield active countdown
        if (isShieldActive && !isScalingUp && !isScalingDown)
        {
            if (PlayerHasWeapon())
            {
                Debug.Log("Shield retracted: Weapon equipped during shield.");
                isScalingDown = true;
                isShieldActive = false;
            }
            else
            {
                shieldTimer += Time.deltaTime;
                if (shieldTimer >= activeDuration)
                {
                    isScalingDown = true;
                    isShieldActive = false;
                }
            }
        }

        // Scale down animation
        if (isScalingDown)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, hiddenScale, Time.deltaTime * scaleSpeed);
            if (Vector3.Distance(transform.localScale, hiddenScale) < 0.01f)
            {
                transform.localScale = hiddenScale;
                isScalingDown = false;

                if (shieldRenderer != null)
                    shieldRenderer.enabled = false;
            }
        }
    }

    private bool PlayerHasWeapon()
    {
        if (player == null) return false;

        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("Weapon"))
                return true;
        }

        return false;
    }
}
