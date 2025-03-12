using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class ArrowSelector : MonoBehaviour
{
    public GameObject playArrow;      // Reference to the Play Arrow
    public GameObject settingsArrow;  // Reference to the Settings Arrow
    public GameObject quitArrow;      // Reference to the Quit Arrow
    private int selectedArrow = 0;    // Which arrow is selected (0 = Play, 1 = Settings, 2 = Quit)

    void Start()
    {
        // Initially, only the play arrow is visible
        playArrow.SetActive(true);
        settingsArrow.SetActive(false);
        quitArrow.SetActive(false);
    }

    void Update()
    {
        // Handle input for up/down arrows
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Move selection up
            selectedArrow = (selectedArrow == 0) ? 2 : selectedArrow - 1;
            UpdateArrowVisibility();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Move selection down
            selectedArrow = (selectedArrow == 2) ? 0 : selectedArrow + 1;
            UpdateArrowVisibility();
        }

        // Handle Enter or Spacebar input
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectOption();
        }
    }

    void UpdateArrowVisibility()
    {
        // Hide all arrows
        playArrow.SetActive(false);
        settingsArrow.SetActive(false);
        quitArrow.SetActive(false);

        // Show the selected arrow
        switch (selectedArrow)
        {
            case 0: // Play Arrow
                playArrow.SetActive(true);
                break;
            case 1: // Settings Arrow
                settingsArrow.SetActive(true);
                break;
            case 2: // Quit Arrow
                quitArrow.SetActive(true);
                break;
        }
    }

    void SelectOption()
    {
        switch (selectedArrow)
        {
            case 0: // Play Arrow - Load SampleScene
                SceneManager.LoadScene("SampleScene");
                break;
            case 1: // Settings Arrow - Load Settings Scene
                SceneManager.LoadScene("Settings");
                break;
            case 2: // Quit Arrow - Exit Game
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Unity Editor
#endif
                break;
        }
    }
}
