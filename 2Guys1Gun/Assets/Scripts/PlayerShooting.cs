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

    private float shotgunCooldown = 1.0f;
    private float shotgunTimer = 0f;

    private float blastCooldown = 1.5f;
    private float blastTimer = 0f;

    private float autoFireRate = 0.075f;
    private float autoFireTimer = 0f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shootKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Attack");
    }

    void Update()
    {
        shotgunTimer -= Time.deltaTime;
        autoFireTimer -= Time.deltaTime;
        blastTimer -= Time.deltaTime;

        if (weaponTransfer == null || !weaponTransfer.hasBall || weaponTransfer.IsWeaponInTransit)
            return;

        string buttonName = itemShopHandler?.lastPressedSlotButton?.name ?? "Button1";

        switch (buttonName)
        {
            case "Button1":
                if (Input.GetKeyDown(shootKey))
                    ShootStandard();
                break;

            case "Button2":
                if (Input.GetKeyDown(shootKey) && shotgunTimer <= 0f)
                {
                    ShootShotgun();
                    shotgunTimer = shotgunCooldown;
                }
                else if (Input.GetKeyDown(shootKey))
                {
                    Debug.Log("Shotgun is on cooldown!");
                }
                break;

            case "Button3":
                if (Input.GetKey(shootKey) && autoFireTimer <= 0f)
                {
                    ShootStandard();
                    autoFireTimer = autoFireRate;
                }
                break;

            case "Button4":
                if (Input.GetKeyDown(shootKey) && blastTimer <= 0f)
                {
                    ShootBlast();
                    blastTimer = blastCooldown;
                }
                break;

            default:
                if (Input.GetKeyDown(shootKey))
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

    private void ShootBlast()
    {
        if (shootableObject == null) return;

        Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * 0.25f);

        GameObject blastProjectile = Instantiate(shootableObject, spawnPosition, Quaternion.identity);

        blastProjectile.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        CircleCollider2D collider = blastProjectile.GetComponent<CircleCollider2D>();
        if (collider != null) collider.radius *= 0.25f;

        Rigidbody2D rb = blastProjectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(direction * shootForce * 0.5f, ForceMode2D.Impulse); // slower speed, half force

        Destroy(blastProjectile, 2f);

        Debug.Log("Blast projectile fired!");
    }
}
