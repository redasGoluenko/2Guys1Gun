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
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            shotCount++;

            if (shotCount >= maxShots)
            {
                if (objectToDestroy != null)
                    Destroy(objectToDestroy);

                Destroy(gameObject);
            }
        }
    }

}
