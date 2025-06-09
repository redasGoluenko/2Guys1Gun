using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int hitsToDestroy = 5;  // Number of hits before destruction
    private int hitCount = 0;

    public PlayerHealth PlayerHealth;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Boss collided with {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            hitCount++;
            Debug.Log($"Hit by PlayerProjectile! Hit count: {hitCount}/{hitsToDestroy}");

            if (hitCount >= hitsToDestroy)
            {
                DestroyBoss();
            }
        }
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            PlayerHealth.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Boss trigger entered by {other.gameObject.name} (Tag: {other.gameObject.tag})");

        if (other.CompareTag("PlayerProjectile"))
        {
            hitCount++;
            Debug.Log($"Hit by PlayerProjectile! Hit count: {hitCount}/{hitsToDestroy}");

            if (hitCount >= hitsToDestroy)
            {
                DestroyBoss();
            }
        }
    }

    private void DestroyBoss()
    {
        Debug.Log("Boss destroyed!");
        Destroy(gameObject);
    }
}
