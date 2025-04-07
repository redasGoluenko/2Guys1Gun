using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// SettingsArrowHandler script to handle the selection of arrow keys for settings
public class SettingsArrowHandler : MonoBehaviour
{
    public GameObject[] leftArrows;
    public GameObject[] rightArrows;
    public TextMeshProUGUI[] leftArrowTexts;
    public TextMeshProUGUI[] rightArrowTexts;

    private int selectedIndex = 6;
    private bool isLeftSide = true;
    private int returnArrowIndex = 6;
    private bool waitingForInput = false;

    private KeyCode[] leftBindings;
    private KeyCode[] rightBindings;

    void Start()
    {      
        leftBindings = new KeyCode[]
        {          
            KeyCode.W,
            KeyCode.S,
            KeyCode.A,
            KeyCode.D,
            KeyCode.LeftShift,
            KeyCode.Space
        };

        rightBindings = new KeyCode[]
        {       
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.KeypadPlus,
            KeyCode.Keypad0
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
     
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {       
            if (selectedIndex == returnArrowIndex)
            {             
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
        for (int i = 0; i < leftArrowTexts.Length; i++)
        {
            leftArrowTexts[i].text = leftBindings[i].ToString();
        }
    
        for (int i = 0; i < rightArrowTexts.Length; i++)
        {
            rightArrowTexts[i].text = rightBindings[i].ToString();
        }
    }
}
