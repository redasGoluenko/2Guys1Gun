using UnityEngine;

// PlayerShooting script to handle the shooting mechanics of the player
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject shootableObject;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private KeyCode shootKey = KeyCode.F;

    private PlayerMovement playerMovement;
    public WeaponTransfer weaponTransfer;

    public bool isLeftPlayer = true;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shootKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Attack");
    }

    void Update()
    {     
        if (Input.GetKeyDown(shootKey))
        {
            if (weaponTransfer != null && weaponTransfer.hasBall && !weaponTransfer.IsWeaponInTransit)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (shootableObject == null) return;

        float offsetDistance = 0.25f;
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