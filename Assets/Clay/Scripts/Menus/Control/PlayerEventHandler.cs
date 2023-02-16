using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerEventHandler : MonoBehaviour
{
    public JoyconManager joyconManager;
    int numJoycons = 0;

    public bool[] playerHasJoined = new bool[4];
    public PlayerInput[] playersJoined = new PlayerInput[4];
    public bool[] playerIsAI = new bool[4]; // note: if playerHasJoined[i]==false, then playerIsAI[i] is a garbage value

    public List<PlayerInput> waitingPlayers = new List<PlayerInput>();

    public PlayerSelectorManager psm;
    public PlayerJoinIconManager iconManager;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("player joined " + playerInput);

        //List<Player> players = new List<Player>(GameObject.FindObjectsOfType<Player>());
        //players.Reverse();
        //GameState.players = players.ToArray();

        int j = -1;
        for (int i = 0; i < 4; i++)
        {
            if (playerHasJoined[i]) continue;
            j = i;
            break;
        }

        if (j == -1) 
        {
            //waitingPlayers.Add(playerInput);
            GameObject.Destroy(playerInput.gameObject);
            return;
        }
        else 
        {
            JoinPlayer(j, playerInput);
        }

        // if (JoyconManager.Instance != null && JoyconManager.Instance.gameObject.active && JoyconManager.Instance.enabled)
        // {
            
        //     //Player[] ps = GameObject.FindObjectsOfType<Player>();
        //     Player p = playerInput.GetComponent<Player>(); //ps[ps.Length-1];

        //     Debug.Log("player might joycon: " + p.name + " ; " + playerInput.currentControlScheme);
        //     if (playerInput.currentControlScheme.StartsWith("Player - Joycon"))
        //     {
        //         p.joycon = JoyconManager.Instance.j[numJoycons++];
        //         Debug.Log("player recieved joycon " + (numJoycons-1));
        //         p.hasJoycon = true;
        //     }
        // }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        int i = 0;
        for (; i < 4; i++)
        {
            // if the current player matches the one that just left
            // or if that functionality is broken, a player that shouldn't be null, 
            // but mysteriously is, is probably the one that just left
            if ((playersJoined[i] == playerInput) || (!playerIsAI[i] && playerHasJoined[i] && playersJoined[i] == null)) 
            { 
                break;
            }
        }
        
        KickPlayer(i, true);

        if (waitingPlayers.Count > 0)
        {
            // playerHasJoined[i] = true;
            // playersJoined[i] = waitingPlayers[0];
            // waitingPlayers.RemoveAt(0);
            // //psm.TogglePlayer(i);

            // link player
            JoinPlayer(i, waitingPlayers[0]);
            waitingPlayers.RemoveAt(0);
        }
    }

    public void JoinAiPlayer(int i)
    {
        JoinPlayer(i, null);
    }

    public void JoinPlayer(int i, PlayerInput playerInput)
    {
        playerIsAI[i] = playerInput == null;
        Player p = (playerInput == null) ? null : playerInput.GetComponent<Player>();
        playerHasJoined[i] = true;
        playersJoined[i] = playerInput;
        iconManager.LinkPlayer(i, p);
        psm.TogglePlayer(i);
        psm.SetComputer(i, playerIsAI[i]);

        if (GameState.players == null) GameState.players = new Player[4];
        GameState.players[i] = playerInput == null ? null : playerInput.GetComponent<Player>();

        if (playerInput != null) psm.SetPlayerInputTypeName(i, playerInput.currentControlScheme);
        
        if (playerInput == null) psm.LockPlayer(i);
    }

    public void KickPlayer(int i)
    {
        KickPlayer(i, false);
    }

    public void KickPlayer(int i, bool playerLeft)
    {
        Debug.Log("kicking player " + i);
        PlayerInput playerInput = playersJoined[i];

        Player p = (playerInput == null) ? null : playerInput.GetComponent<Player>();
        iconManager.UnlinkPlayer(i, p);
        playersJoined[i] = null;
        playerHasJoined[i] = false;
        psm.TogglePlayer(i);
        psm.UnlockPlayer(i);

        if (p != null && p.hasJoycon != null)
        {
            numJoycons--;
        }

        if (!playerLeft && playerInput != null)
        {
            Destroy(playerInput.gameObject);
        }
    }

    public Player[] GetPlayersJoined()
    {
        Player[] players = new Player[4];

        for (int i = 0; i < 4; i++)
        {
            if (playersJoined[i] == null) continue;
            players[i] = playersJoined[i].GetComponent<Player>();
        }

        return players;
    }
}
