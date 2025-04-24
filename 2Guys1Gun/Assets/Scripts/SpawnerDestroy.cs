using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerDestroy : MonoBehaviour
{
    // Public GameObject that can be assigned in the Inspector
    public GameObject objectToDestroy;

    // Counter to track the number of hits
    private int shotCount = 0;

    // Number of shots before destruction
    public int maxShots = 10;

    // This is called when a collision with another object occurs
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object colliding has the "PlayerProjectile" tag
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Increment shot count
            shotCount++;

            // If the object has been hit the maximum number of times
            if (shotCount >= maxShots)
            {
                // Destroy the assigned object
                if (objectToDestroy != null)
                {
                    Destroy(objectToDestroy);
                }

                // Destroy the spawner object itself
                Destroy(gameObject);
            }
        }
    }
}
