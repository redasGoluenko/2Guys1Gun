using System.Collections;
using Pathfinding.Ionic.Zip;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 8.0f;
    private float jumpingPower = 16.0f;
    private float horizontal;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask secondaryGroundLayer;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode switchKey;
    public Animator Animation;
    public DialogueTypewriter dialogueTypewriter;
    public ShieldHandler shieldHandler;
    public AudioSource jumpAudioSource;

    [Header("Initial Facing Direction")]
    public bool facingRightOnStart = true;

    [Header("Player Side")]
    public bool isLeftPlayer = true;
    private ItemShopHandler itemShopHandler;
    private WeaponTransfer wt;
    void Start()
    {
        // Initialize default keys if not already set
        if (!PlayerPrefs.HasKey("L_Jump") || !PlayerPrefs.HasKey("R_Jump"))
        {
            InputFieldKeyBinder.ResetAllKeysToDefault();
        }
        PlayerShooting shootingScript = GetComponent<PlayerShooting>();
        if (shootingScript != null)
        {
            itemShopHandler = shootingScript.itemShopHandler;
            wt = shootingScript.weaponTransfer;
        }
        // Assign saved keys
        jumpKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Jump");
        leftKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Left");
        rightKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Right");

        isFacingRight = facingRightOnStart;
        Vector3 localScale = transform.localScale;
        if (!isFacingRight)
        {
            localScale.x = Mathf.Abs(localScale.x) * -1f;
        }
        else
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
    }

    void Update()
    {
        // Skip slot selection if weapon is transferring
        if (!wt.IsWeaponInTransit && wt.hasBall && itemShopHandler.lastPressedSlotButton != null)
        {
            SetActiveSlot(itemShopHandler.lastPressedSlotButton.name);
        }
        else
        {
            ResetAllSlots();
            Animation.SetFloat("Speed", 0);
        }

        // Speed boost from shield
        speed = shieldHandler.isShieldActive ? 12.0f : 8.0f;

        if (dialogueTypewriter.dialogueActive)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        horizontal = 0f;
        if (Input.GetKey(leftKey)) horizontal = -1f;
        else if (Input.GetKey(rightKey)) horizontal = 1f;      
        // Jump
        if (Input.GetKeyDown(jumpKey) && isGrounded())
        {
            jumpAudioSource.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        else if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();

        Animation.SetFloat("Speed", Mathf.Abs(horizontal));
        Animation.SetFloat("VerticalVelocity", rb.velocity.y);
    }


    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer | secondaryGroundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }
    private void SetActiveSlot(string slotName)
    {
        ResetAllSlots();

        switch (slotName)
        {
            case "Button1":
                Animation.SetBool("Slot1", true);
                break;
            case "Button2":
                Animation.SetBool("Slot2", true);
                break;
            case "Button3":
                Animation.SetBool("Slot3", true);
                break;
            case "Button4":
                Animation.SetBool("Slot4", true);
                break;
            default:
                break;
        }
    }

    private void ResetAllSlots()
    {
        Animation.SetBool("Slot1", false);
        Animation.SetBool("Slot2", false);
        Animation.SetBool("Slot3", false);
        Animation.SetBool("Slot4", false);
    }

}
