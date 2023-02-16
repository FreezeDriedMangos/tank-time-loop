using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoinIconManager : MonoBehaviour
{
    private int numPlayers = 0;
    [SerializeField] private List<PlayerJoinIcon> joinIcons; 

    // public void PlayerJoined()
    // {
    //     joinIcons[numPlayers].SetColor(true);
    //     GameState.players[numPlayers].onInputNotify = joinIcons[numPlayers].InputResponse;

    //     numPlayers++;
    // }

    // public void PlayerKicked()
    // {
    //     joinIcons[--numPlayers].SetColor(false);
    // }

    public void LinkPlayer(int player, Player p)
    {
        joinIcons[player].SetColor(true);
        if (p != null)
            p.onInputNotify = joinIcons[player].InputResponse;
    }

    public void UnlinkPlayer(int player, Player p)
    {
        joinIcons[player].SetColor(false);
        if (p != null)
            p.onInputNotify = null;
    }
}
