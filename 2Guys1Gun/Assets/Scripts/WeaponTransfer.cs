using System.Collections;
using UnityEngine;

public class WeaponTransfer : MonoBehaviour
{
    [SerializeField] private GameObject transferableObject;
    [SerializeField] private float transferSpeed = 100f;
    [SerializeField] private KeyCode transferKey = KeyCode.T;
    public bool hasBall = false;
    public WeaponTransfer otherPlayer;

    private Coroutine activeTransferCoroutine;

    // Public property to check if the weapon is currently in transit (optional, for debugging)
    public bool IsWeaponInTransit => activeTransferCoroutine != null;

    void Update()
    {
        // Allow transfer even if weapon is in transit
        if (Input.GetKeyDown(transferKey) && transferableObject != null)
        {
            // Stop any ongoing transfer
            if (activeTransferCoroutine != null)
            {
                StopCoroutine(activeTransferCoroutine);
            }

            // Set hasBall to false for both players when transfer starts
            hasBall = false;
            if (otherPlayer != null)
            {
                otherPlayer.hasBall = false;
            }
            // Start a new transfer to this player
            activeTransferCoroutine = StartCoroutine(TransferObject());
        }
    }

    private IEnumerator TransferObject()
    {
        Debug.Log($"{gameObject.name}: Transfer started, weapon in transit");
        // Start from the weapon's current position
        Vector3 startPos = transferableObject.transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPos, transform.position) / transferSpeed;

        // Move the weapon to this player's position
        while (elapsedTime < duration)
        {
            transferableObject.transform.position = Vector3.Lerp(startPos, transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final position and parent
        transferableObject.transform.position = transform.position;
        transferableObject.transform.SetParent(transform);

        // Update hasBall only when the weapon arrives
        hasBall = true;
        if (otherPlayer != null)
        {
            otherPlayer.hasBall = false;
        }

        // Clear the active coroutine
        activeTransferCoroutine = null;
    }
}