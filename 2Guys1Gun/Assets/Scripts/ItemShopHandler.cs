using UnityEngine;
using System.Collections;

public class ItemShopHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemShopObject;

    private void Start()
    {
        if (itemShopObject != null)
        {
            itemShopObject.SetActive(false); // Ensure it's off at start
            StartCoroutine(CheckForSpawners());
        }
        else
        {
            Debug.LogWarning("ItemShopActivator: No item shop object assigned.");
        }
    }

    IEnumerator CheckForSpawners()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");

            if (spawners.Length == 0)
            {
                itemShopObject.SetActive(true);
                Debug.Log("Item shop enabled: no spawners found.");
                yield break;
            }
        }
    }
}
