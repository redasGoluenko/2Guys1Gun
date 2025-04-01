using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

public class WeaponSlot : MonoBehaviour
{
    public WeaponTransfer weaponTransfer;
    public Image weaponIcon; // UI Image for weapon visibility

    // Start is called before the first frame update
    void Start()
    {
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(false); // Hide icon initially
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(weaponTransfer.hasBall); // Show if hasBall is true
        }
    }
}
