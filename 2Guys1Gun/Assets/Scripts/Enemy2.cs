using UnityEngine;
using System.Collections;

// Enemy2 script to handle the enemy's movement and collision detection
public class Enemy2 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float destroyRange = 1f;
    [SerializeField] private float timeBeforeDestroy = 5f;

    public PlayerHealth playerHealth;

    private void Start()
    {      
        StartCoroutine(DestroyEnemy());
    }

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
        Vector3 direction = (player.transform.position - transform.position).normalized;
   
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void CheckAndDestroyPlayer()
    {       
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
      
        if (distanceToPlayer <= destroyRange)
        {
            playerHealth.GameOver();
            Debug.Log("Player destroyed!");
        }
    }

    private IEnumerator DestroyEnemy()
    {       
        yield return new WaitForSeconds(timeBeforeDestroy);
      
        Destroy(gameObject);
        Debug.Log("Enemy destroyed after " + timeBeforeDestroy + " seconds!");
    }
}
