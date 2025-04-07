using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SceneLoader script to handle scene loading
public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public void LoadScene(string sceneName)
    {       
        SceneManager.LoadScene(sceneName);
    }
}
