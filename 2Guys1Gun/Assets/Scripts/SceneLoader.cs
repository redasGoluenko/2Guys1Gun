using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SceneLoader script to handle scene loading
public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public bool resetPrefs = true;
    public void LoadScene(string sceneName)
    {
        if(resetPrefs)
        {
            ResetPlayerPrefs();
        }
        
        SceneManager.LoadScene(sceneName);
    }
    private void ResetPlayerPrefs()
    {
        Debug.Log("Resetting PlayerPrefs for scene: " + sceneName);
        PlayerPrefs.DeleteKey("LastPressedSlotButtonName");        
        PlayerPrefs.SetInt("PlayerSoulCount", 0);
        PlayerPrefs.DeleteKey("PlayerSoulCount");
        PlayerPrefs.Save();       
    }
}
