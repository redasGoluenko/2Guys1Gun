using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  // Add this for scene management

public class SettingsArrowHandler : MonoBehaviour
{
    public GameObject[] leftArrows;   // Left player arrows
    public GameObject[] rightArrows;  // Right player arrows
    public TextMeshProUGUI[] leftArrowTexts;  // Left player key text
    public TextMeshProUGUI[] rightArrowTexts; // Right player key text

    private int selectedIndex = 6;    // Start with Return Arrow selected
    private bool isLeftSide = true;   // Start on Left Player column
    private int returnArrowIndex = 6; // Return arrow index
    private bool waitingForInput = false; // True if waiting for key input

    private KeyCode[] leftBindings;   // Key bindings for left player
    private KeyCode[] rightBindings;  // Key bindings for right player

    void Start()
    {
        // Set default key bindings (adjusted based on your provided controls)
        leftBindings = new KeyCode[]
        {
            //KeyCode.Escape,  // Return
            KeyCode.W,       // Jump
            KeyCode.S,       // Down
            KeyCode.A,       // Left
            KeyCode.D,       // Right
            KeyCode.LeftShift, // Attack
            KeyCode.Space    // Switch Weapon
        };

        rightBindings = new KeyCode[]
        {
            //KeyCode.Escape,   // Return (same as left)
            KeyCode.UpArrow,  // Jump
            KeyCode.DownArrow, // Down
            KeyCode.LeftArrow, // Left
            KeyCode.RightArrow, // Right
            KeyCode.KeypadPlus,  // Attack
            KeyCode.Keypad0    // Switch Weapon
        };

        UpdateArrowVisibility();
        UpdateTextDisplay();
    }

    void Update()
    {
        if (waitingForInput)
        {
            DetectKeyInput();
            return;
        }

        // Handle vertical movement (Up/Down)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex == 0) ? leftArrows.Length - 1 : selectedIndex - 1;
            UpdateArrowVisibility();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex == leftArrows.Length - 1) ? 0 : selectedIndex + 1;
            UpdateArrowVisibility();
        }

        // Handle horizontal movement (Left/Right)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isLeftSide = true;
            UpdateArrowVisibility();
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && selectedIndex != returnArrowIndex)
        {
            isLeftSide = false;
            UpdateArrowVisibility();
        }

        // Start keybinding process
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // Check if Return Arrow is selected
            if (selectedIndex == returnArrowIndex)
            {
                // Load the "User Interface" scene when Return arrow is selected and Enter/Space is pressed
                SceneManager.LoadScene("User Interface");
            }
            else
            {
                waitingForInput = true;
            }
        }
    }

    void DetectKeyInput()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                if (isLeftSide)
                    leftBindings[selectedIndex] = key;
                else
                    rightBindings[selectedIndex] = key;

                waitingForInput = false;
                UpdateTextDisplay();
                return;
            }
        }
    }

    void UpdateArrowVisibility()
    {
        foreach (GameObject arrow in leftArrows) arrow.SetActive(false);
        foreach (GameObject arrow in rightArrows) arrow.SetActive(false);

        if (isLeftSide)
            leftArrows[selectedIndex].SetActive(true);
        else
            rightArrows[selectedIndex].SetActive(true);
    }

    void UpdateTextDisplay()
    {
        // Update text for left side
        for (int i = 0; i < leftArrowTexts.Length; i++)
        {
            leftArrowTexts[i].text = leftBindings[i].ToString();
        }

        // Update text for right side
        for (int i = 0; i < rightArrowTexts.Length; i++)
        {
            rightArrowTexts[i].text = rightBindings[i].ToString();
        }
    }
}
