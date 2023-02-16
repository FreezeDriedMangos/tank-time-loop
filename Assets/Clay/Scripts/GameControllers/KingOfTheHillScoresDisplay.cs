using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingOfTheHillScoresDisplay : MonoBehaviour
{
    public Slider[] scoreDisplays;

    public float[] scores;
    
    public VsGameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        if (GameState.gameplayType != GameState.GameplayType.KING_OF_THE_HILL)
            this.enabled = false;
        else
        {
            for (int i = 0; i < 4; i++)
                scoreDisplays[i].gameObject.SetActive(GameState.playerJoined[i]);
        }
    } 

    // Update is called once per frame
    void Update()
    {
        scores = GameState.kingOfTheHillScores;
        
        // float totalScore = 0;
        // foreach (float score in GameState.kingOfTheHillScores) totalScore += score;

        float totalScore = gameController.roundLength_seconds * gameController.numRounds;

        if (GameState.config.miscConfig.kothModeScoreBasedOnTotalScore)
        {
            totalScore = 0;
            foreach (float score in GameState.kingOfTheHillScores) totalScore += score;
        }

        if (totalScore == 0) totalScore = 1;

        for (int i = 0; i < 4; i++)
        {
            scoreDisplays[i].value = GameState.kingOfTheHillScores[i] / totalScore;
        }
    }
}
