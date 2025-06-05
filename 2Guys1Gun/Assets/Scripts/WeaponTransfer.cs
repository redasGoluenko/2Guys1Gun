using System.Collections;
using System.Globalization;
using UnityEngine;

// WeaponTransfer script to handle the transfer of weapons between players
public class WeaponTransfer : MonoBehaviour
{
    [SerializeField] private GameObject transferableObject;
    [SerializeField] private float transferSpeed = 100f;
    [SerializeField] private KeyCode transferKey = KeyCode.T;
    public bool hasBall = false;
    public WeaponTransfer otherPlayer;
    Renderer weaponRenderer;

    private Coroutine activeTransferCoroutine;

    public bool isLeftPlayer = true; // Indicates if this is the left player

    public bool IsWeaponInTransit => activeTransferCoroutine != null;

    void Start()
    {
        weaponRenderer = transferableObject.GetComponent<Renderer>();
        weaponRenderer.enabled = false; // Hide the weapon initially
        transferKey = InputFieldKeyBinder.GetSavedKey(!isLeftPlayer, "Switch");
    }
    void Update()
    {      
        if (Input.GetKeyDown(transferKey) && transferableObject != null)
        {          
            if (activeTransferCoroutine != null)
            {
                StopCoroutine(activeTransferCoroutine);
            }
       
            hasBall = false;
            if (otherPlayer != null)
            {
                otherPlayer.hasBall = false;
            }         
            activeTransferCoroutine = StartCoroutine(TransferObject());
        }
    }

    private IEnumerator TransferObject()
    {
        weaponRenderer.enabled = true; // Ensure the weapon is visible during transfer
        Debug.Log($"{gameObject.name}: Transfer started, weapon in transit");    
        Vector3 startPos = transferableObject.transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPos, transform.position) / transferSpeed;
       
        while (elapsedTime < duration)
        {
            transferableObject.transform.position = Vector3.Lerp(startPos, transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        weaponRenderer.enabled = false; // Hide the weapon after transfer

        transferableObject.transform.position = transform.position;
        transferableObject.transform.SetParent(transform);
        
        hasBall = true;
        if (otherPlayer != null)
        {
            otherPlayer.hasBall = false;
        }
       
        activeTransferCoroutine = null;
    }
}