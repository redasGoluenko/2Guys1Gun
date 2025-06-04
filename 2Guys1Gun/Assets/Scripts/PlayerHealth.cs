using UnityEngine;
using System.Collections;

// PlayerHealth script to manage the player's health, damage, and game over state
public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] public int lavaDamage = 0;
    [SerializeField] public float lavaDamageRate = 0.2f;
    private bool isTakingLavaDamage = false;

    public GameObject gameOverUI;
    public ShieldHandler ShieldHandler;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;

        Time.timeScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {       
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                Debug.Log("Hit by enemy");
                TakeDamage(enemy.damage);
            }
        }
        else if (collision.CompareTag("Lava"))
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
            TakeDamage(lavaDamage);
            yield return new WaitForSeconds(lavaDamageRate);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!ShieldHandler.isShieldActive)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateColor();

            if (currentHealth <= 0)
            {
                GameOver();
            }
        }       
    }

    private void UpdateColor()
    {
        float healthRatio = (float)currentHealth / maxHealth;

        float minBrightness = 0.3f;
        float brightness = Mathf.Lerp(minBrightness, 1f, healthRatio);

        spriteRenderer.color = new Color(
            originalColor.r * brightness,
            originalColor.g * brightness,
            originalColor.b * brightness,
            originalColor.a
        );
    }

    public void GameOver()
    {            
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Time.timeScale = 0;
    }
}
