using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] public int lavaDamage = 0;
    [SerializeField] public float lavaDamageRate = 0.2f;
    private bool isTakingLavaDamage = false;

    // Reference to the UI GameObject that will show the game over screen
    public GameObject gameOverUI;

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
            // Get the EnemyBase component and retrieve its damage value
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
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateColor();

        if (currentHealth <= 0)
        {
            GameOver();
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

    private void GameOver()
    {            
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Time.timeScale = 0;
    }
}
