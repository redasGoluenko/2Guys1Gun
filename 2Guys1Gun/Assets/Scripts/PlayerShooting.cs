using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject shootableObject;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private KeyCode shootKey = KeyCode.F;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (shootableObject == null) return;

        float offsetDistance = 0.5f;
        Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * offsetDistance);

        GameObject copy = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
        copy.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);

        CircleCollider2D collider = copy.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius *= 0.125f;
        }

        Rigidbody2D copyRb = copy.GetComponent<Rigidbody2D>();
        if (copyRb != null)
        {
            copyRb.AddForce(direction * shootForce, ForceMode2D.Impulse);
        }

        Destroy(copy, 1f);
    }
}
