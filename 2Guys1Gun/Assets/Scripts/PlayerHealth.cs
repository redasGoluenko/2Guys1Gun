using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float darknessLevel = 1f; // Start at full brightness
    public float reduceAmount = 0.2f;

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
            //destroy all objects tagged "Player"
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                Destroy(player);
            }
        }
    }
}
