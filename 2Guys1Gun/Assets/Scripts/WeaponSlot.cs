using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

// WeaponSlot script to manage the weapon slot UI
public class WeaponSlot : MonoBehaviour
{
    public WeaponTransfer weaponTransfer;
    public Image weaponIcon;
   
    void Start()
    {
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(false); // Hide icon initially
        }
    }
   
    void Update()
    {
        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(weaponTransfer.hasBall); // Show if hasBall is true
        }
    }
}
