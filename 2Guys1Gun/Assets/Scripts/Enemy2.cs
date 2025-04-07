using UnityEngine;
using System.Collections;  // Needed for using Coroutines

public class FollowAndDestroy : MonoBehaviour
{
    [SerializeField] private GameObject player;  // Reference to the player GameObject
    [SerializeField] private float moveSpeed = 3f;  // Speed at which the object moves toward the player
    [SerializeField] private float destroyRange = 1f;  // Range at which the object destroys the player
    [SerializeField] private float timeBeforeDestroy = 5f;  // Time before enemy is destroyed after the start of the scene

    public PlayerHealth playerHealth;  // Reference to the PlayerHealth script

    private void Start()
    {
        // Start the enemy's destroy countdown when the scene starts
        StartCoroutine(DestroyEnemy());
    }

    private void Update()
    {
        if (player != null)
        {
            // Move towards the player
            MoveTowardsPlayer();

            // Check if within range and destroy the player
            CheckAndDestroyPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Move the object towards the player
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void CheckAndDestroyPlayer()
    {
        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // If the object is within the destroy range, destroy the player
        if (distanceToPlayer <= destroyRange)
        {
            playerHealth.GameOver();
            Debug.Log("Player destroyed!");
        }
    }

    private IEnumerator DestroyEnemy()
    {
        // Wait for the specified time before destroying the enemy
        yield return new WaitForSeconds(timeBeforeDestroy);

        // Destroy this enemy object
        Destroy(gameObject);
        Debug.Log("Enemy destroyed after " + timeBeforeDestroy + " seconds!");
    }
}
