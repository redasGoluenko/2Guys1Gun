using UnityEngine;

// Projectile script to handle the projectile's behavior and collision detection
public class Projectile : MonoBehaviour
{
    public int damage = 25;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float distance = rb.velocity.magnitude * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
