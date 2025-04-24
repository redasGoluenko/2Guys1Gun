using System.Collections;
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

    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    public Animator Animation;
    public DialogueTypewriter dialogueTypewriter;

    [Header("Initial Facing Direction")]
    public bool facingRightOnStart = true;

    void Start()
    {
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
        if (dialogueTypewriter.dialogueActive)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // Freeze horizontal movement
            Animation.SetFloat("Speed", 0);
            return;
        }

        horizontal = 0f;

        if (Input.GetKey(leftKey))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(rightKey))
        {
            horizontal = 1f;
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f)
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
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
}
