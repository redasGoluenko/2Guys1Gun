using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 8.0f;
    private float jumpingPower = 16.0f;
    private bool isFacingRight = true;
    private float horizontal;

    public PlayerMovement otherPlayer;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode transferKey = KeyCode.T;
    [SerializeField] private KeyCode shootKey = KeyCode.F; // Key for shooting the ball

    [SerializeField] private GameObject transferableObject;
    [SerializeField] private GameObject shootableObject; // New serialized field for the object to be shot
    [SerializeField] private float transferSpeed = 5f;
    private bool isTransferring = false;
    public bool hasBall = false;
    [SerializeField] private float shootForce = 10f;

    void Update()
    {
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

        if (Input.GetKeyDown(transferKey) && transferableObject != null && !isTransferring)
        {
            hasBall = true;
            otherPlayer.hasBall = false;
            StartCoroutine(TransferObject());
        }

        // Shooting the ball or other object
        if (Input.GetKeyDown(shootKey) && hasBall && shootableObject != null)
        {
            Shoot();
        }

        Flip();
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

    private IEnumerator TransferObject()
    {
        isTransferring = true;
        Vector3 startPos = transferableObject.transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPos, transform.position) / transferSpeed;

        while (elapsedTime < duration)
        {        
            transferableObject.transform.position = Vector3.Lerp(startPos, transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        transferableObject.transform.position = transform.position;
        transferableObject.transform.SetParent(transform);
        isTransferring = false;
    }

    private void Shoot()
    {
        // Instantiate the shootable object at the player's position
        GameObject copy = Instantiate(shootableObject, transform.position, Quaternion.identity);

        // Scale down the object to 0.125x of its original size
        copy.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);

        // Adjust the collider's radius to match the new scale
        CircleCollider2D collider = copy.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            // Scale down the radius to match the object's scale (0.125x in this case)
            collider.radius *= 0.125f;
        }

        // Apply force to the copy in the direction the player is facing
        Rigidbody2D copyRb = copy.GetComponent<Rigidbody2D>();
        if (copyRb != null)
        {
            // Use the player's facing direction to determine the shooting direction
            Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
            copyRb.AddForce(direction * shootForce, ForceMode2D.Impulse);
        }

        // Destroy the projectile after 1 second
        Destroy(copy, 1f);  // 'copy' is the projectile, and 1f is the delay (in seconds)
    }



}
