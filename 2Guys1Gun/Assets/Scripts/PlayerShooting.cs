using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject shootableObject;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private KeyCode shootKey = KeyCode.F;

    private PlayerMovement playerMovement;
    public WeaponTransfer weaponTransfer;
    public ItemShopHandler itemShopHandler;

    public bool isLeftPlayer = true;

    private float cooldownTimer = 0f;
    private float shotgunCooldown = 1.0f; // Cooldown in seconds for shotgun

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shootKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Attack");
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shootKey))
        {
            if (weaponTransfer != null && weaponTransfer.hasBall && !weaponTransfer.IsWeaponInTransit)
            {
                HandleShooting();
            }
        }
    }

    private void HandleShooting()
    {
        if (itemShopHandler == null || itemShopHandler.lastPressedSlotButton == null)
        {
            ShootStandard();
            return;
        }

        string buttonName = itemShopHandler.lastPressedSlotButton.name;
        Debug.Log($"Last pressed button: {buttonName}");

        switch (buttonName)
        {
            case "Button1":
                ShootStandard();
                break;

            case "Button2":
                if (cooldownTimer <= 0f)
                {
                    ShootShotgun();
                    cooldownTimer = shotgunCooldown;
                }
                else
                {
                    Debug.Log("Shotgun is on cooldown!");
                }
                break;

            default:
                ShootStandard();
                break;
        }
    }

    private void ShootStandard()
    {
        if (shootableObject == null) return;

        Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * 0.25f);

        GameObject copy = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
        copy.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);

        CircleCollider2D collider = copy.GetComponent<CircleCollider2D>();
        if (collider != null) collider.radius *= 0.125f;

        Rigidbody2D rb = copy.GetComponent<Rigidbody2D>();
        if (rb != null) rb.AddForce(direction * shootForce, ForceMode2D.Impulse);

        Destroy(copy, 1f);
    }

    private void ShootShotgun()
    {
        if (shootableObject == null) return;

        int pelletCount = 5;
        float spreadAngle = 20f;
        float baseAngle = playerMovement.IsFacingRight() ? 0f : 180f;

        Vector3 spawnPosition = transform.position + (Vector3)((playerMovement.IsFacingRight() ? Vector2.right : Vector2.left) * 0.25f);

        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = spreadAngle * ((float)i / (pelletCount - 1) - 0.5f);
            float angle = baseAngle + angleOffset;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject pellet = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
            pellet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            CircleCollider2D collider = pellet.GetComponent<CircleCollider2D>();
            if (collider != null) collider.radius *= 0.1f;

            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.AddForce(direction.normalized * shootForce, ForceMode2D.Impulse);

            Destroy(pellet, 1f);
        }

        Debug.Log("Shotgun fired!");
    }
}
