using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemShopHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemShopObject;

    public Button slot1;
    public Button slot2;
    public Button slot3;
    public Button slot4;
    public Button slot5;
    public Button slot6;
    public Button slot7;
    public Button slot8;
    public Button slot9;
    public Button slot10;

    public Button lastPressedSlotButton;

    private Coroutine spawnerCheckCoroutine;

    private void Start()
    {
        lastPressedSlotButton = slot1; // Default to slot1
        if (itemShopObject != null)
        {
            itemShopObject.SetActive(false); // Ensure it's off at start
            spawnerCheckCoroutine = StartCoroutine(CheckForSpawners());
        }
        else
        {
            Debug.LogWarning("ItemShopActivator: No item shop object assigned.");
        }

        RegisterButton(slot1);
        RegisterButton(slot2);
        RegisterButton(slot3);
        RegisterButton(slot4);
        RegisterButton(slot5);
        RegisterButton(slot6);
        RegisterButton(slot7);
        RegisterButton(slot8);
        RegisterButton(slot9);
        RegisterButton(slot10);
    }

    private void RegisterButton(Button button)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => OnSlotButtonPressed(button));
        }
    }

    private void OnSlotButtonPressed(Button button)
    {
        lastPressedSlotButton = button;
        Debug.Log($"Button {button.name} was pressed.");
    }

    private void Update()
    {
        // Toggle item shop visibility with Numpad+
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (itemShopObject != null)
            {
                bool isActive = itemShopObject.activeSelf;
                itemShopObject.SetActive(!isActive);

                // Optionally stop the coroutine if manually toggled on
                if (!isActive && spawnerCheckCoroutine != null)
                {
                    StopCoroutine(spawnerCheckCoroutine);
                    spawnerCheckCoroutine = null;
                    Debug.Log("Spawner check coroutine stopped due to manual toggle.");
                }

                Debug.Log($"Item shop toggled to {(itemShopObject.activeSelf ? "ON" : "OFF")} with Numpad+.");
            }
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
