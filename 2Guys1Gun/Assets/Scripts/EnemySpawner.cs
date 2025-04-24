using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;  // Reference to the enemy prefab
    public float spawnInterval = 10f;  // Interval between spawns
    public string targetString;  // Public variable to specify the target string in the inspector
    public DialogueTypewriter DialogueTypewriter;  // Reference to the DialogueTypewriter script

    private bool hasStartedSpawning = false;

    void Update()
    {
        // Check if the dialogue is not active and the spawning hasn't started yet
        if (!DialogueTypewriter.dialogueActive && !hasStartedSpawning)
        {
            InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
            hasStartedSpawning = true;
        }
    }

    void SpawnEnemy()
    {
        // Instantiate the enemy at the spawner's position
        GameObject spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity);

        // Access the Enemy script attached to the spawned enemy
        Enemy2 enemyScript = spawnedEnemy.GetComponent<Enemy2>();

        if (enemyScript != null)
        {
            // Set the target string to the value specified in the Inspector
            enemyScript.target = targetString;
        }
        else
        {
            Debug.LogWarning("Enemy script not found on spawned enemy.");
        }
    }
}
