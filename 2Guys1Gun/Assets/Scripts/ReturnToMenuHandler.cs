using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenuHandler : MonoBehaviour
{
    void Update()
    {     
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the "User Interface" scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("User Interface");
        }

    }
}
