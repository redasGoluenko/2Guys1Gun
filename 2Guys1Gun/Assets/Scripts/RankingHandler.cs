using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingHandler : MonoBehaviour
{
    public SceneCounterManager sceneCounterManager;

    public Image rank1;
    public Image rank2;
    public Image rank3;

    private int highscore;

    // Start is called before the first frame update
    void Start()
    {
        highscore = sceneCounterManager.highestScore;
        UpdateRankVisibility();
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

    // Optional: if you want to check and update ranks every frame
    // void Update()
    // {
    //     // If highscore can change during runtime and you want dynamic update:
    //     // highscore = sceneCounterManager.highestScore;
    //     // UpdateRankVisibility();
    // }
}
