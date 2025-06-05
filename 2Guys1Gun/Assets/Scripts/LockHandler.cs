using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockHandler : MonoBehaviour
{
    public int soulCost = 1; // Amount to deduct
    private SoulCounterHandler soulCounterHandler;
    public TMP_Text lockText; // Optional text to display lock status

    private void Start()
    {
        GameObject soulCounterObject = GameObject.FindGameObjectWithTag("SoulCounter");
        if (soulCounterObject != null)
        {
            soulCounterHandler = soulCounterObject.GetComponent<SoulCounterHandler>();
            if (soulCounterHandler == null)
            {
                Debug.LogError("SoulCounterHandler script not found on SoulCounter object.");
            }
        }
        else
        {
            Debug.LogError("No object with tag 'SoulCounter' found in the scene.");
        }
    }
    private void Update()
    {
        // Update lock text if it exists
        if (lockText != null)
        {
            lockText.text = $"Unlock Cost: {soulCost} Souls";
        }
    }

    // Call this method to attempt to unlock
    public void TryUnlock()
    {
        if (soulCounterHandler == null)
        {
            Debug.LogError("SoulCounterHandler reference is missing.");
            return;
        }

        if (soulCounterHandler.souls >= soulCost)
        {
            soulCounterHandler.souls -= soulCost;

            // Save updated soul count
            PlayerPrefs.SetInt("PlayerSoulCount", soulCounterHandler.souls);
            PlayerPrefs.Save();

            soulCounterHandler.RefreshSoulCounter(); // Update UI
            Destroy(gameObject); // Unlock
        }
        else
        {
            Debug.Log("Not enough souls to unlock.");
        }
    }

}
