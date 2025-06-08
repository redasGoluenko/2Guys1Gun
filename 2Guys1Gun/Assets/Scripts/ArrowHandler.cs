using UnityEngine;
using UnityEngine.SceneManagement;

// Arrow Selector script to handle the selection of arrows in the main menu
public class ArrowSelector : MonoBehaviour
{
    public GameObject playArrow;     
    public GameObject settingsArrow;
    public GameObject quitArrow; 
    public AudioSource arrowAudioSource;
    public AudioSource tabSelectAudioSource;
    private int selectedArrow = 0;
    private float sceneStartTime;

    private bool hasStarted = false;

    void Start()
    {
        playArrow.SetActive(true);
        settingsArrow.SetActive(false);
        quitArrow.SetActive(false);

        PlayerPrefs.DeleteKey("LastPressedSlotButtonName");
        PlayerPrefs.DeleteKey("PlayerSoulCount");
        PlayerPrefs.Save();

        sceneStartTime = Time.time; // record start time

        // Don't play audio on initial setup
        hasStarted = true;
    }


    void Update()
    {      
        // Check for input to change the selected arrow
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {         
            selectedArrow = (selectedArrow == 0) ? 2 : selectedArrow - 1;
            UpdateArrowVisibility();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {           
            selectedArrow = (selectedArrow == 2) ? 0 : selectedArrow + 1;
            UpdateArrowVisibility();
        }
     
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectOption();
        }
    }

    // Update the visibility of the arrows based on the selected option
    void UpdateArrowVisibility()
    {
        playArrow.SetActive(false);
        settingsArrow.SetActive(false);
        quitArrow.SetActive(false);

        switch (selectedArrow)
        {
            case 0:
                playArrow.SetActive(true);
                break;
            case 1:
                settingsArrow.SetActive(true);
                break;
            case 2:
                quitArrow.SetActive(true);
                break;
        }

        // Play audio only if 0.5 seconds passed AND it's not the initial setup
        if (arrowAudioSource != null && Time.time - sceneStartTime > 0.5f && hasStarted)
        {
            arrowAudioSource.Play();
        }
    }



    // Load the selected scene or quit the application
    void SelectOption()
    {
        tabSelectAudioSource.Play();
        switch (selectedArrow)
        {
            case 0:
                SceneManager.LoadScene("Level1");
                break;
            case 1:
                SceneManager.LoadScene("Settings");
                break;
            case 2:
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Unity Editor
#endif
                break;
        }
    }
}
