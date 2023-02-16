using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinData : MonoBehaviour
{
    public Text winner;
    public bool isTie = false;

    void Awake() 
    {
        if (GameState.gameplayType == GameState.GameplayType.KING_OF_THE_HILL)
        {
            float max = 0;
            foreach(float a in GameState.kingOfTheHillScores)
            {
                max = Mathf.Max(a, max);
            }

            List<int> winners = new List<int>();
            for (int i = 0; i < 4; i ++)
            {
                if (GameState.kingOfTheHillScores[i] == max)
                    winners.Add(i+1);
            }

            GameState.winners = winners.ToArray();
        } 

        if (GameState.winners != null && GameState.winners.Length >= 1)
        {
            if (GameState.winners.Length == 1)
                winner.text = isTie ? "Self tie: Player " + GameState.winners[0] : "Player " + GameState.winners[0] + " wins!";
            else if (GameState.winners.Length == 2)
                winner.text = "Players " + GameState.winners[0] + " and " + GameState.winners[1] + " win!";
            else
            {
                winner.text = "Players ";
                for (int i = 0; i < GameState.winners.Length-1; i++)
                {
                    winner.text += GameState.winners[i] + ", ";
                }

                winner.text += "and " + GameState.winners[GameState.winners.Length-1] + (isTie? "!" : " win!");
            }
        }   

        
    }
}
