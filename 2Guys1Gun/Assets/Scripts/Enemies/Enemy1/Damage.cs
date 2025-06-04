using UnityEngine;

public class Damage : MonoBehaviour
{
    private EnemyBase enemyBase;
    public float damageCooldown = 0.5f;
    private float lastDamageTime = -Mathf.Infinity;
    private void Start()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(enemyBase.targetPlayer.tag))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && enemyBase != null)
                {
                    Debug.Log("Took damage");
                    enemyBase.DealDamage(playerHealth);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}
