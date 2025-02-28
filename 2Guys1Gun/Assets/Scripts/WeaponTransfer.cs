using System.Collections;
using UnityEngine;

public class WeaponTransfer : MonoBehaviour
{
    [SerializeField] private GameObject transferableObject;
    [SerializeField] private float transferSpeed = 5f;
    [SerializeField] private KeyCode transferKey = KeyCode.T;
    public bool hasBall = false;
    public WeaponTransfer otherPlayer;

    private bool isTransferring = false;

    void Update()
    {
        if (Input.GetKeyDown(transferKey) && transferableObject != null && !isTransferring)
        {
            hasBall = true;
            if (otherPlayer != null)
            {
                otherPlayer.hasBall = false;
            }
            StartCoroutine(TransferObject());
        }
    }

    private IEnumerator TransferObject()
    {
        isTransferring = true;
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
        isTransferring = false;
    }
}
