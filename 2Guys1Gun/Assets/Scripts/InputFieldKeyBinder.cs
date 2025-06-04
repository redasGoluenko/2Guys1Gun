using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class InputFieldKeyBinder : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField displayText;
    public bool isLeftPlayer = true;

    public bool isJump;
    public bool isDown;
    public bool isLeft;
    public bool isRight;
    public bool isAttack;
    public bool isSwitch;

    private bool waitingForKey = false;
    private Coroutine flashRoutine;

    private string prefsKey => $"{(isLeftPlayer ? "L" : "R")}_{GetActionName()}";

    void Start()
    {
        displayText.textComponent.color = Color.white;
        if (displayText == null)
        {
            Debug.LogError("DisplayText (TMP_InputField) not assigned.");
            return;
        }

        displayText.readOnly = true; // prevent manual typing

        KeyCode savedKey = GetSavedKey();
        displayText.text = savedKey.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!waitingForKey && displayText != null)
        {
            waitingForKey = true;
            displayText.text = "Press key...";
            flashRoutine = StartCoroutine(FlashText());
        }
    }

    void OnGUI()
    {
        if (!waitingForKey) return;

        Event e = Event.current;
        if (e.isKey)
        {
            KeyCode newKey = e.keyCode;

            if (KeyAlreadyUsed(newKey))
            {
                displayText.text = $"[{newKey}] in use!";
                StopFlashing();
                StartCoroutine(RevertAfterDelay(1f));
            }
            else
            {
                PlayerPrefs.SetString(prefsKey, newKey.ToString());
                PlayerPrefs.Save();
                displayText.text = newKey.ToString();
                StopFlashing();
            }

            waitingForKey = false;
        }
    }

    void StopFlashing()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        if (displayText != null)
            displayText.textComponent.color = Color.white;

    }

    IEnumerator FlashText()
    {
        while (true)
        {
            if (displayText != null)
                displayText.textComponent.color = displayText.textComponent.color == Color.white ? Color.yellow : Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator RevertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (displayText != null)
            displayText.text = GetSavedKey().ToString();
    }

    private string GetActionName()
    {
        if (isJump) return "Jump";
        if (isDown) return "Down";
        if (isLeft) return "Left";
        if (isRight) return "Right";
        if (isAttack) return "Attack";
        if (isSwitch) return "Switch";
        return "Unknown";
    }

    private KeyCode GetDefaultKey()
    {
        if (isLeftPlayer)
        {
            if (isJump) return KeyCode.W;
            if (isDown) return KeyCode.S;
            if (isLeft) return KeyCode.A;
            if (isRight) return KeyCode.D;
            if (isAttack) return KeyCode.Space;
            if (isSwitch) return KeyCode.LeftShift;
        }
        else
        {
            if (isJump) return KeyCode.UpArrow;
            if (isDown) return KeyCode.DownArrow;
            if (isLeft) return KeyCode.LeftArrow;
            if (isRight) return KeyCode.RightArrow;
            if (isAttack) return KeyCode.Keypad0;
            if (isSwitch) return KeyCode.KeypadPlus;
        }
        return KeyCode.None;
    }

    private KeyCode GetSavedKey()
    {
        string saved = PlayerPrefs.GetString(prefsKey, GetDefaultKey().ToString());
        if (System.Enum.TryParse(saved, out KeyCode result))
            return result;
        return GetDefaultKey();
    }

    private bool KeyAlreadyUsed(KeyCode key)
    {
        string[] actions = { "Jump", "Down", "Left", "Right", "Attack", "Switch" };

        foreach (string action in actions)
        {
            string otherKey = PlayerPrefs.GetString($"{(isLeftPlayer ? "L" : "R")}_{action}", "");
            if (!string.IsNullOrEmpty(otherKey) && otherKey == key.ToString() && action != GetActionName())
            {
                return true;
            }
        }
        return false;
    }

    public static KeyCode GetSavedKey(bool isLeftPlayer, string action)
    {
        string key = PlayerPrefs.GetString($"{(isLeftPlayer ? "L" : "R")}_{action}", KeyCode.None.ToString());
        if (System.Enum.TryParse(key, out KeyCode result))
            return result;
        return KeyCode.None;
    }

    public static void ResetAllKeysToDefault()
    {
        SetDefaults(true);
        SetDefaults(false);
        PlayerPrefs.Save();
    }

    private static void SetDefaults(bool isLeft)
    {
        void Set(string action, KeyCode key)
        {
            PlayerPrefs.SetString($"{(isLeft ? "L" : "R")}_{action}", key.ToString());
        }

        if (isLeft)
        {
            Set("Jump", KeyCode.W);
            Set("Down", KeyCode.S);
            Set("Left", KeyCode.A);
            Set("Right", KeyCode.D);
            Set("Attack", KeyCode.Space);
            Set("Switch", KeyCode.LeftShift);
        }
        else
        {
            Set("Jump", KeyCode.UpArrow);
            Set("Down", KeyCode.DownArrow);
            Set("Left", KeyCode.LeftArrow);
            Set("Right", KeyCode.RightArrow);
            Set("Attack", KeyCode.Keypad0);
            Set("Switch", KeyCode.KeypadPlus);
        }
    }
}
