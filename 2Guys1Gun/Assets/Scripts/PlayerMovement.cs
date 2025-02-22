using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 8.0f;
    private float jumpingPower = 16.0f;
    private bool isFacingRight = true;
    private float horizontal;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode transferKey = KeyCode.T;

    [SerializeField] private GameObject transferableObject;
    [SerializeField] private float transferSpeed = 5f;
    private bool isTransferring = false;

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
            StartCoroutine(TransferObject());
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
            // Update end position dynamically to keep up with player movement
            transferableObject.transform.position = Vector3.Lerp(startPos, transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position matches player's position
        transferableObject.transform.position = transform.position;
        transferableObject.transform.SetParent(transform); // Parent the object to the player
        isTransferring = false;
    }

}
