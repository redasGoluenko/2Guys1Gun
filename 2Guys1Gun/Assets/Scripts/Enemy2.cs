using UnityEngine;

// Enemy2 script to handle the enemy's movement and collision detection in 2D
public class Enemy2 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float destroyRange = 1f;

    public PlayerHealth playerHealth;

    private void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
            CheckAndDestroyPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void CheckAndDestroyPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= destroyRange)
        {
            playerHealth.GameOver();
            Debug.Log("Player destroyed!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject); // Destroy this enemy           
            Debug.Log("Enemy hit by projectile and destroyed!");
        }
    }
}
