using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KotHData : MonoBehaviour
{
    public Text text;

    
    void Awake() 
    {
        if (GameState.gameplayType == GameState.GameplayType.KING_OF_THE_HILL)
        {
            text.text = "Player 1: " + GameState.kingOfTheHillScores[0] + "s\n" +
                        "Player 2: " + GameState.kingOfTheHillScores[1] + "s\n" +
                        "Player 3: " + GameState.kingOfTheHillScores[2] + "s\n" +
                        "Player 4: " + GameState.kingOfTheHillScores[3] + "s";
        }
    }
}
