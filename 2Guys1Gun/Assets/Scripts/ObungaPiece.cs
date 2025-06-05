using UnityEngine;

public class ObungaPiece : MonoBehaviour
{
    public ObungaHandler handler;
    public int index; // Set this in the inspector or via handler
    private int hitCount = 0;
    private int hitsToDestroy = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile") && handler.CanBeDamaged(index))
        {
            Destroy(collision.gameObject); // Optional
            hitCount++;

            if (hitCount >= hitsToDestroy)
            {
                handler.DestroyObunga(index);
                Destroy(gameObject);
            }
        }
    }
}
