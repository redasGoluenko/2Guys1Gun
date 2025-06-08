using UnityEngine;

public class BossProjectileHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamageIfPlayer(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    private void DealDamageIfPlayer(GameObject obj)
    {
        if (obj.CompareTag("Player1") || obj.CompareTag("Player2"))
        {
            PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                Debug.Log($"Damaged {obj.name} for 10 HP.");
            }
            else
            {
                Debug.LogWarning($"No PlayerHealth script found on {obj.name}.");
            }
        }
    }
}
