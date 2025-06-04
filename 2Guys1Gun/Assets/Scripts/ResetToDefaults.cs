using UnityEngine;

public class ResetToDefaults : MonoBehaviour
{
    public void ResetToDefault()
    {
        InputFieldKeyBinder.ResetAllKeysToDefault();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // refresh
    }
}
