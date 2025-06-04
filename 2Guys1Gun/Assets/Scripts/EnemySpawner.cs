using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemy;               // Enemy prefab
    public float spawnInterval = 10f;      // Time between spawns
    public GameObject target;              // Player target GameObject

    [Header("Override Stats (EnemyBase only)")]
    public float health = 100f;
    public float speed = 2f;
    public int damage = 10;

    [Header("Dialogue Dependency")]
    public DialogueTypewriter DialogueTypewriter;

    private bool hasStartedSpawning = false;

    void Update()
    {
        if (!DialogueTypewriter.dialogueActive && !hasStartedSpawning)
        {
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
            hasStartedSpawning = true;
        }
    }

    void SpawnEnemy()
    {
        if (enemy == null)
        {
            Debug.LogWarning("Enemy prefab not assigned.");
            return;
        }

        GameObject spawned = Instantiate(enemy, transform.position, Quaternion.identity);

        EnemyBase baseScript = spawned.GetComponent<EnemyBase>();
        if (baseScript != null)
        {
            baseScript.maxHealth = health;
            baseScript.moveSpeed = speed;
            baseScript.damage = damage;
            baseScript.targetPlayer = target;
            return;
        }

        Enemy2 enemy2 = spawned.GetComponent<Enemy2>();
        if (enemy2 != null)
        {
            enemy2.player = target;
            return;
        }

        Debug.LogWarning("Spawned enemy does not match any known type.");
    }
}
