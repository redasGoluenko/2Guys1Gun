using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UI namespace

// WeaponSlot script to manage the weapon slot UI
public class WeaponSlot : MonoBehaviour
{
    public WeaponTransfer weaponTransfer;  
    public ItemShopHandler itemShopHandler;
    public Image Blue;
    public Image Green;
    public Image Red;
    public Image Yellow;

    void Update()
    {
        // Hide all images first
        Blue.gameObject.SetActive(false);
        Green.gameObject.SetActive(false);
        Red.gameObject.SetActive(false);
        Yellow.gameObject.SetActive(false);

        // Show the correct image based on the selected button
        if (itemShopHandler.lastPressedSlotButton.name == "Button1")
        {
            Red.gameObject.SetActive(weaponTransfer.hasBall);
        }
        else if (itemShopHandler.lastPressedSlotButton.name == "Button2")
        {
            Blue.gameObject.SetActive(weaponTransfer.hasBall);
        }
        else if (itemShopHandler.lastPressedSlotButton.name == "Button3")
        {
            Green.gameObject.SetActive(weaponTransfer.hasBall);
        }
        else if (itemShopHandler.lastPressedSlotButton.name == "Button4")
        {
            Yellow.gameObject.SetActive(weaponTransfer.hasBall);
        }
    }

}
