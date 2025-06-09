using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingHandler : MonoBehaviour
{
    private SceneCounterManager sceneCounterManager;

    public Image rank1;
    public Image rank2;
    public Image rank3;

    private int highscore;

    // Start is called before the first frame update
    void Start()
    {
        GameObject managerObj = GameObject.FindGameObjectWithTag("SceneCounterManager");
        if (managerObj != null)
        {
            sceneCounterManager = managerObj.GetComponent<SceneCounterManager>();
            if (sceneCounterManager != null)
            {
                highscore = sceneCounterManager.highestScore;
                UpdateRankVisibility();
            }
            else
            {
                Debug.LogError("SceneCounterManager component not found on the tagged object.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with tag 'SceneCounterManager'.");
        }
    }


    private void UpdateRankVisibility()
    {
        // Hide all first
        rank1.gameObject.SetActive(false);
        rank2.gameObject.SetActive(false);
        rank3.gameObject.SetActive(false);

        // Show only the matching rank
        if (highscore == 5)
        {
            rank1.gameObject.SetActive(true);
        }
        else if (highscore == 10)
        {
            rank2.gameObject.SetActive(true);
        }
        else if (highscore == 15)
        {
            rank3.gameObject.SetActive(true);
        }
    }
}
