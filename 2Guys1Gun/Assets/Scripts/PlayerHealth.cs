using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float darknessLevel = 1f; // Start at full brightness
    public float reduceAmount = 0.2f;
    public float lavaDamageRate = 0.5f; // Damage interval in seconds
    private bool isTakingLavaDamage = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Store the original color
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Collision detected");
            DarkenObject(); // Make the object darker
        }
        if (collision.CompareTag("Lava"))
        {
            Debug.Log("Lava detected");
            if (!isTakingLavaDamage)
            {
                isTakingLavaDamage = true;
                StartCoroutine(TakeLavaDamage());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
        {
            Debug.Log("Exited lava");
            isTakingLavaDamage = false;
        }
    }

    private IEnumerator TakeLavaDamage()
    {
        while (isTakingLavaDamage)
        {
            DarkenObject(); // Apply damage effect
            yield return new WaitForSeconds(lavaDamageRate); // Wait before applying damage again
        }
    }

    private void DarkenObject()
    {
        darknessLevel -= reduceAmount; // Reduce brightness (adjust step size as needed)
        darknessLevel = Mathf.Clamp(darknessLevel, 0f, 1f); // Keep within bounds

        // Apply the darkness level while maintaining original color proportions
        spriteRenderer.color = new Color(
            originalColor.r * darknessLevel,
            originalColor.g * darknessLevel,
            originalColor.b * darknessLevel,
            originalColor.a // Keep alpha unchanged
        );

        if (darknessLevel <= 0f)
        {
            DestroyAllPlayers();
        }
    }

    private void DestroyAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
    }
}
