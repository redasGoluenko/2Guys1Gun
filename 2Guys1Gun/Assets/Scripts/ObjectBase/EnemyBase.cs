using UnityEngine;

// EnemyBase script to handle the enemy's health, movement, and damage
public class EnemyBase : MonoBehaviour
{
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public int damage = 10;
    public GameObject targetPlayer;
    protected float currentHealth;
    protected SpriteRenderer spriteRenderer;
    private Color originalColor;
    protected float darknessLevel = 1f;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (targetPlayer == null)
        {
            Debug.LogWarning("No target player assigned to the enemy.");
        }

    }
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            int damageAmount = other.GetComponent<Projectile>().damage;
            TakeDamage(damageAmount);
        }
    }

    public virtual void DealDamage(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(damage);
    }

    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        DarkenObject();

        if (currentHealth <= 0)
        {
            Debug.Log("Enemy destroyed!!!!!!");
            GameObject soulCounterObject = GameObject.FindGameObjectWithTag("SoulCounter");
            if (soulCounterObject != null)
            {
                SoulCounterHandler counter = soulCounterObject.GetComponent<SoulCounterHandler>();
                if (counter != null)
                {
                    counter.UpdateSoulCounter();
                }
            }
            Destroy(gameObject);

        }
    }

    private void DarkenObject()
    {
        darknessLevel -= 0.2f;
        darknessLevel = Mathf.Clamp(darknessLevel, 0f, 1f);
        spriteRenderer.color = new Color(originalColor.r * darknessLevel, originalColor.g * darknessLevel, originalColor.b * darknessLevel, originalColor.a);
    }

    public virtual void Move()
    {
        if (targetPlayer != null)
        {
            Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
}
