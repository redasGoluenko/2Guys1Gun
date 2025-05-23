using System.Collections;
using UnityEngine;

// WeaponTransfer script to handle the transfer of weapons between players
public class WeaponTransfer : MonoBehaviour
{
    [SerializeField] private GameObject transferableObject;
    [SerializeField] private float transferSpeed = 100f;
    [SerializeField] private KeyCode transferKey = KeyCode.T;
    public bool hasBall = false;
    public WeaponTransfer otherPlayer;

    private Coroutine activeTransferCoroutine;

    public bool IsWeaponInTransit => activeTransferCoroutine != null;

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