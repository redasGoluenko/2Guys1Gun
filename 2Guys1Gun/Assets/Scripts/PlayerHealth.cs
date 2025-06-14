using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] public int lavaDamage = 0;
    [SerializeField] public float lavaDamageRate = 0.2f;
    private bool isTakingLavaDamage = false;

    public GameObject gameOverUI;
    public ShieldHandler ShieldHandler;
    public AudioSource deathAudioSource;
    private bool deathAudioPlayed = false;

    private const string HealthKey = "PlayerCurrentHealth";

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Restore saved health if available, otherwise use max
        if (PlayerPrefs.HasKey(HealthKey))
        {
            currentHealth = PlayerPrefs.GetInt(HealthKey);
        }
        else
        {
            currentHealth = maxHealth;
        }

        UpdateColor();
        Time.timeScale = 1;
    }

    private void OnApplicationQuit()
    {
        // Optional: clear saved health on quit
        PlayerPrefs.DeleteKey(HealthKey);
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
            PlayerPrefs.SetInt(HealthKey, currentHealth); // Save updated health

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
        if (!ShieldHandler.isShieldActive)
        {
            if (deathAudioSource != null && !deathAudioSource.isPlaying && !deathAudioPlayed)
            {
                deathAudioSource.Play();
                deathAudioPlayed = true;
            }
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }

            PlayerPrefs.DeleteKey(HealthKey); // Optional: reset health after game over
            //unparent camera from all objects tagged "Player1" and "Player2" and set active to false          
            DeactivatePlayers();
            Time.timeScale = 0;
        }        
    }
    void DeactivatePlayers()
    {
        string[] tags = { "Player1", "Player2" };

        foreach (string tag in tags)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    // Unparent the player's camera (if any)
                    Camera playerCam = player.GetComponentInChildren<Camera>();
                    if (playerCam != null)
                    {
                        playerCam.transform.parent = null;
                    }

                    // Deactivate the player
                    player.SetActive(false);
                }
            }
        }
    }
}
