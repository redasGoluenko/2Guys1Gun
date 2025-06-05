using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject shootableObject;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private KeyCode shootKey = KeyCode.F;

    private PlayerMovement playerMovement;
    public WeaponTransfer weaponTransfer;
    public ItemShopHandler itemShopHandler;
    public TMP_Text magazine;

    public bool isLeftPlayer = true;

    private float shotgunCooldown = 1.0f;
    private float shotgunTimer = 0f;

    private float blastCooldown = 1.5f;
    private float blastTimer = 0f;

    private float autoFireRate = 0.075f;
    private float autoFireTimer = 0f;

    // Magazine + Reloading
    private int maxAmmo = 15;
    private int currentAmmo;
    private bool isReloading = false;
    private KeyCode reloadKey;

    // Auto-fire tracking
    private int autoShotsFired = 0;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        shootKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Attack");
        reloadKey = InputFieldKeyBinder.GetSavedKey(isLeftPlayer, "Down");

        currentAmmo = maxAmmo;
        UpdateMagazineUI();
    }

    void Update()
    {
        shotgunTimer -= Time.deltaTime;
        autoFireTimer -= Time.deltaTime;
        blastTimer -= Time.deltaTime;

        if (weaponTransfer == null || !weaponTransfer.hasBall || weaponTransfer.IsWeaponInTransit)
            return;

        string buttonName = itemShopHandler?.lastPressedSlotButton?.name ?? "Button1";

        if (buttonName == "Button1")
        {
            // Handle reloading
            if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
                return;
            }

            // Prevent shooting while reloading or out of ammo
            if (Input.GetKeyDown(shootKey) && !isReloading && currentAmmo > 0)
            {
                ShootStandard();
                currentAmmo--;
                UpdateMagazineUI();
            }

            if (Input.GetKeyDown(shootKey) && currentAmmo <= 0 && !isReloading)
            {
                Debug.Log("Out of ammo! Press reload key.");
            }

            return; // Exit early since we're handling everything in this block
        }

        switch (buttonName)
        {
            case "Button2":
                if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
                {
                    StartCoroutine(Reload());
                    return;
                }

                if (Input.GetKeyDown(shootKey))
                {
                    if (isReloading)
                    {
                        Debug.Log("Reloading... can't shoot!");
                    }
                    else if (currentAmmo >= 5 && shotgunTimer <= 0f)
                    {
                        ShootShotgun();
                        currentAmmo -= 5;
                        UpdateMagazineUI();
                        shotgunTimer = shotgunCooldown;
                    }
                    else if (currentAmmo < 5)
                    {
                        Debug.Log("Not enough ammo for shotgun! Press reload key.");
                    }
                    else
                    {
                        Debug.Log("Shotgun is on cooldown!");
                    }
                }
                break;

            case "Button3":
                if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
                {
                    StartCoroutine(Reload());
                    return;
                }

                if (Input.GetKey(shootKey))
                {
                    if (isReloading)
                    {
                        Debug.Log("Reloading... can't shoot!");
                    }
                    else if (currentAmmo > 0 && autoFireTimer <= 0f)
                    {
                        ShootStandard();
                        autoShotsFired++;

                        if (autoShotsFired >= 3)
                        {
                            currentAmmo--;
                            autoShotsFired = 0;
                            UpdateMagazineUI();
                        }

                        autoFireTimer = autoFireRate;
                    }
                    else if (currentAmmo <= 0)
                    {
                        Debug.Log("Out of ammo! Press reload key.");
                    }
                }
                break;

            case "Button4":
                if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
                {
                    StartCoroutine(Reload());
                    return;
                }

                if (Input.GetKeyDown(shootKey))
                {
                    if (isReloading)
                    {
                        Debug.Log("Reloading... can't shoot!");
                    }
                    else if (currentAmmo >= 7 && blastTimer <= 0f)
                    {
                        ShootBlast();
                        currentAmmo -= 7;
                        UpdateMagazineUI();
                        blastTimer = blastCooldown;
                    }
                    else if (currentAmmo < 7)
                    {
                        Debug.Log("Not enough ammo for blast! Press reload key.");
                    }
                    else
                    {
                        Debug.Log("Blast is on cooldown!");
                    }
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
            rb.AddForce(direction * shootForce * 0.5f, ForceMode2D.Impulse);

        Destroy(blastProjectile, 2f);

        Debug.Log("Blast projectile fired!");
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        magazine.text = "Reloading...";
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(1.5f);

        currentAmmo = maxAmmo;
        autoShotsFired = 0; // Reset auto-fire tracking
        UpdateMagazineUI();
        isReloading = false;
        Debug.Log("Reload complete.");
    }

    private void UpdateMagazineUI()
    {
        magazine.text = $".../{currentAmmo}";
    }
}
