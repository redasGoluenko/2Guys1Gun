using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float spawnInterval = 10f;
    public DialogueTypewriter DialogueTypewriter;

    private bool hasStartedSpawning = false;

    void Update()
    {
        if (!DialogueTypewriter.dialogueActive && !hasStartedSpawning)
        {
            InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
            hasStartedSpawning = true;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemy, transform.position, Quaternion.identity);
    }
}
