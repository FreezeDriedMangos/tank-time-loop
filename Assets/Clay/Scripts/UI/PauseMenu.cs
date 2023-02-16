using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject[] menuVariants;
    private int playerPaused = 0;
    private bool paused = false;

    public MenuNavigator nav;
    public GameObject[] menuCursors;

    // Start is called before the first frame update
    void Start()
    {
        //nav = FindObjectOfType<MenuNavigator>();
        foreach(GameObject g in menuCursors) g.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        Pause(0);
    }

    public void Pause(int playerNum)
    {
        for(int i = 0; i < menuVariants.Length; i++)
        {
            menuVariants[i].SetActive(false);
        }

        Debug.Log("WHATS THE PLAYERNUM " + playerNum);
        Debug.Log("# menu variants " + menuVariants.Length);

        menuVariants[playerNum].SetActive(true);
        playerPaused = playerNum;
        paused = true;
        nav.enabled = true;
        foreach(GameObject g in menuCursors) g.SetActive(true);
    }

    public void Unpause(int playerNum)
    {
        Debug.Log("hi, yeah so I'm supposed to unpause but I guess I just don't want to.");
        if (playerNum != playerPaused) return;

        for(int i = 0; i < menuVariants.Length; i++)
        {
            menuVariants[i].SetActive(false);
        }

        paused = false;
        nav.enabled = false;
        foreach(GameObject g in menuCursors) g.SetActive(false);
    }

    public void ForceUnpause()
    {
        Debug.Log("hi, yeah so I'm being forced to unpause but I guess I just don't want to.");
        for(int i = 0; i < menuVariants.Length; i++)
        {
            menuVariants[i].SetActive(false);
        }

        paused = false;
        nav.enabled = false;
    }

    public void TogglePaused(int playerNum)
    {
        if (paused)
        {
            Unpause(playerNum);
        }
        else
        {
            Pause(playerNum);
        }
    }
    
    public void TogglePaused(Player p)
    {
        int playerNum;
        for(playerNum = 0; playerNum < GameState.players.Length; playerNum++)
        {
            if (GameState.players[playerNum] == p) break;
        }

        if (playerNum >= GameState.players.Length) return;

        TogglePaused(playerNum);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("don't want to");
        GameState.gameplayType = GameState.GameplayType.MENUS;
        GameState.menuState = GameState.MenuState.NORMAL;
        SceneManager.LoadScene("Menus");
    }
}
