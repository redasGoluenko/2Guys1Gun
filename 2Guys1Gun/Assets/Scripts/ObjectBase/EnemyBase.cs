using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float maxHealth = 100f;   // Default health
    public float moveSpeed = 2f;     // Default Movement speed
    public int damage = 10;          // Default damage

    protected float currentHealth; 
    protected SpriteRenderer spriteRenderer;
    private Color originalColor;
    protected float darknessLevel = 1f;

    protected virtual void Start()
    {
        currentHealth = maxHealth; // Sets healh to maxHealth
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        originalColor = spriteRenderer.color;
    }

    // Function to deal damage to the player
    public void DealDamage(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(damage);
    }

    // Function to take damage of the name
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;  // Reduces enemy health value by damageAmount
        DarkenObject();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);  // Destroy the enemy when health reaches 0
        }
    }

    // Apply the darkening effect based on health
    private void DarkenObject()
    {
        darknessLevel -= 0.2f;  // Darken as health decreases
        darknessLevel = Mathf.Clamp(darknessLevel, 0f, 1f);
        spriteRenderer.color = new Color(originalColor.r * darknessLevel, originalColor.g * darknessLevel, originalColor.b * darknessLevel, originalColor.a);
    }

    // Default movement function
    public virtual void Move()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime * 0f);
    }
}
