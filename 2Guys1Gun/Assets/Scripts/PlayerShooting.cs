using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject shootableObject;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private KeyCode shootKey = KeyCode.F;
    [SerializeField] private float verticalOffset = 0f; // Bullet height offset

    private PlayerMovement playerMovement;
    public WeaponTransfer weaponTransfer;
    public ItemShopHandler itemShopHandler;
    public TMP_Text magazine;
    public AudioSource shootingAudioSource;
    public AudioSource reloadAudioSource;

    public bool isLeftPlayer = true;

    private float shotgunCooldown = 1.0f;
    private float shotgunTimer = 0f;

    private float blastCooldown = 1.5f;
    private float blastTimer = 0f;

    private float autoFireRate = 0.075f;
    private float autoFireTimer = 0f;

    private int maxAmmo = 15;
    private int currentAmmo;
    private bool isReloading = false;
    private KeyCode reloadKey;

    private int autoShotsFired = 0;
    private float flipX;

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
        flipX = playerMovement.IsFacingRight() ? -1f : 1f;
        if (!isReloading && currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        shotgunTimer -= Time.deltaTime;
        autoFireTimer -= Time.deltaTime;
        blastTimer -= Time.deltaTime;

        if (weaponTransfer == null || !weaponTransfer.hasBall || weaponTransfer.IsWeaponInTransit)
            return;

        string buttonName = itemShopHandler?.lastPressedSlotButton?.name ?? "Button1";

        if (buttonName == "Button1")
        {
            if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
                return;
            }

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

            return;
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
        shootingAudioSource.Play();
        if (shootableObject == null) return;

        Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * 0.25f) + new Vector3(0, verticalOffset, 0);

        GameObject copy = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
        copy.transform.localScale = new Vector3(2f * flipX, 2f, 2f);

        CircleCollider2D collider = copy.GetComponent<CircleCollider2D>();
        if (collider != null) collider.radius *= 0.125f;

        Rigidbody2D rb = copy.GetComponent<Rigidbody2D>();
        if (rb != null) rb.AddForce(direction * shootForce, ForceMode2D.Impulse);    
        Destroy(copy, 1f);
    }

    private void ShootShotgun()
    {
        shootingAudioSource.Play();
        if (shootableObject == null) return;

        int pelletCount = 5;
        float spreadAngle = 20f;
        float baseAngle = playerMovement.IsFacingRight() ? 0f : 180f;
        Vector3 spawnPosition = transform.position + (Vector3)((playerMovement.IsFacingRight() ? Vector2.right : Vector2.left) * 0.25f) + new Vector3(0, verticalOffset, 0);

        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = spreadAngle * ((float)i / (pelletCount - 1) - 0.5f);
            float angle = baseAngle + angleOffset;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject pellet = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
            pellet.transform.localScale = new Vector3(2f * flipX, 2f, 2f);

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
        shootingAudioSource.Play();
        if (shootableObject == null) return;

        Vector2 direction = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)(direction * 0.25f) + new Vector3(0, verticalOffset, 0);

        GameObject blastProjectile = Instantiate(shootableObject, spawnPosition, Quaternion.identity);
        blastProjectile.transform.localScale = new Vector3(4f * flipX, 4f, 4f);

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
        reloadAudioSource.Play();
        isReloading = true;
        magazine.text = "Reloading...";
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(1.5f);

        currentAmmo = maxAmmo;
        autoShotsFired = 0;
        UpdateMagazineUI();
        isReloading = false;
        Debug.Log("Reload complete.");
        StartCoroutine(FadeOutAudio(reloadAudioSource, 0.5f)); // 0.5 seconds fade
    }

    private void UpdateMagazineUI()
    {
        magazine.text = $".../{currentAmmo}";
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop(); // Stop after fade
        audioSource.volume = startVolume; // Reset volume for next use
    }

}
