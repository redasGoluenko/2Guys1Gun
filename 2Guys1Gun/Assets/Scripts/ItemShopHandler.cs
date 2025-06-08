using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemShopHandler : MonoBehaviour
{
    [SerializeField] private GameObject itemShopObject;

    public Button slot1;
    public Button slot2;
    public Button slot3;
    public Button slot4;
    public Button slot6;
    public Button slot7;
    public Button slot8;
    public Button slot9;
    public Button slot10;   

    public Button lastPressedSlotButton;

    private Coroutine objectCheckCoroutine;
    private const string LastPressedKey = "LastPressedSlotButtonName";

    private void Start()
    {
        // Delete PlayerPrefs key if needed
        string savedButtonName = PlayerPrefs.GetString(LastPressedKey, "Button1");
        lastPressedSlotButton = GetButtonByName(savedButtonName) ?? slot1;

        RegisterButton(slot1);
        RegisterButton(slot2);
        RegisterButton(slot3);
        RegisterButton(slot4);
        RegisterButton(slot6);
        RegisterButton(slot7);
        RegisterButton(slot8);
        RegisterButton(slot9);
        RegisterButton(slot10);

        if (itemShopObject != null)
        {
            itemShopObject.SetActive(false);

            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == "Level1")
                objectCheckCoroutine = StartCoroutine(CheckForObjectsWithTag("Spawner"));
            else if (sceneName == "Level2")
                objectCheckCoroutine = StartCoroutine(CheckForObjectsWithTag("Obunga"));
        }
        else
        {
            Debug.LogWarning("ItemShopHandler: No item shop object assigned.");
        }
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
        PlayerPrefs.SetString(LastPressedKey, button.name);
        PlayerPrefs.Save();
    }

    private Button GetButtonByName(string name)
    {
        switch (name)
        {
            case "Button1": return slot1;
            case "Button2": return slot2;
            case "Button3": return slot3;
            case "Button4": return slot4;
            case "Button6": return slot6;
            case "Button7": return slot7;
            case "Button8": return slot8;
            case "Button9": return slot9;
            case "Button10": return slot10;
            default: return null;
        }
    }

    IEnumerator CheckForObjectsWithTag(string tag)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            if (objects.Length == 0)
            {
                itemShopObject.SetActive(true);
                Debug.Log($"Item shop enabled: no '{tag}' objects found.");
                yield break;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey(LastPressedKey);
        PlayerPrefs.Save();
    }
}
