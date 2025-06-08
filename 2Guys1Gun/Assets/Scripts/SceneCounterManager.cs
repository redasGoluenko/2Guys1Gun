using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCounterManager : MonoBehaviour
{
    public static SceneCounterManager Instance;

    public int currentScore = 0;
    public int highestScore = 0;

    private const string HighestScoreKey = "HighestScore";

    private void Awake()
    {
        // Singleton pattern: Only one instance persists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load highest score from PlayerPrefs
        highestScore = PlayerPrefs.GetInt(HighestScoreKey, 0);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            currentScore++;
            Debug.Log($"Level1 loaded. Current Score: {currentScore}");
        }
        else if (scene.name == "User Interface")
        {
            if (currentScore > highestScore)
            {
                highestScore = currentScore;
                PlayerPrefs.SetInt(HighestScoreKey, highestScore);
                PlayerPrefs.Save();
                Debug.Log($"New High Score: {highestScore}");
            }

            currentScore = 0;
            Debug.Log("User Interface loaded. Score reset.");
        }
    }
}
