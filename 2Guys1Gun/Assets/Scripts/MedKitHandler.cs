using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKitHandler : MonoBehaviour
{
    public PlayerHealth player1Health; // Reference to the player's health script
    public PlayerHealth player2Health; // Reference to the second player's health script

    public void HealPlayer()
    {
        player1Health.currentHealth = player1Health.maxHealth; // Restore player's health to maximum
        player2Health.currentHealth = player2Health.maxHealth; // Restore player's health to maximum
    }
}
