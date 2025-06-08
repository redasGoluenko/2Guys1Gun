using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicController : MonoBehaviour
{
    private static MenuMusicController instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop music when entering Level1 or other scenes
        if (scene.name == "Level1" || scene.name == "Level2") // change this to your actual game scene
        {
            Destroy(gameObject); // optional: stop music when game starts
        }
    }
}
