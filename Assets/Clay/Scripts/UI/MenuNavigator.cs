using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuNavigator : MonoBehaviour
{
    public Button[] buttons;
    Dictionary<Player, int> players = new Dictionary<Player, int>();
    Dictionary<Player, Selectable> playerSelected = new Dictionary<Player, Selectable>();
    //Dictionary<Player, int> playerSelectedButton = new Dictionary<Player, int>();
    Dictionary<Player, int> playerCursor = new Dictionary<Player, int>();
    List<Player> allPlayers = new List<Player>();
    
    public List<GameObject> cursors = new List<GameObject>();
    public Selectable lastSelected;

    // Start is called before the first frame update
    void Start()
    {
        //buttons = Resources.FindObjectsOfTypeAll<Button>();//GameObject.FindObjectsOfType<Button>();//new List<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        //buttons[0].navigation
        
        foreach (Player p in allPlayers)
        {
            if (playerSelected[p] == null || !playerSelected[p].IsActive())
                SetPlayerSelection(p, GameObject.FindObjectOfType<Selectable>()); 
        }
    }

    public void Select(Player p)
    {
        if (!CheckNewPlayer(p)) return;
        if (playerSelected[p] == null || !playerSelected[p].IsActive()) { SetPlayerSelection(p, GameObject.FindObjectOfType<Selectable>()); return; }

        Debug.Log("PLAYER SELECTED " + players[p] + " activating " + playerSelected[p]);

        //buttons[playerSelectedButton[p]].onClick.Invoke();
        //playerSelected[p].OnPointerDown(null);
        //playerSelected[p].OnPointerUp(null);
        
        if (playerSelected[p] is Button) (playerSelected[p] as Button).onClick.Invoke();
        if (playerSelected[p] is Toggle) (playerSelected[p] as Toggle).isOn = !(playerSelected[p] as Toggle).isOn;
        
    }

    private Selectable FindFirst()
    {
        //return GameObject.FindObjectOfType<Selectable>();
        Button b = GameObject.FindObjectOfType<Button>();
        Selectable s = b as Selectable;
        Selectable s2 = GameObject.FindObjectOfType<Selectable>();
        Debug.Log("first selectable " + b + " selectable "  + s + " first real selectable " + s2);
        return s2;
    }

    public void MoveSelection(Player p, Vector2 v)
    {
        if (!CheckNewPlayer(p)) return;
        if (playerSelected[p] == null || !playerSelected[p].gameObject.activeInHierarchy) { Debug.Log(playerSelected[p] + " was not valid!"); SetPlayerSelection(p, FindFirst()); return; }

        Debug.Log("PLAYER MOVED " + players[p] + " from " + playerSelected[p]);

        if (false) {}
        else if (v.x > 0) { Debug.Log("Moving to the Right"); SetPlayerSelection(p, playerSelected[p].FindSelectableOnRight()); }
        else if (v.x < 0) { Debug.Log("Moving to the Left"); SetPlayerSelection(p, playerSelected[p].FindSelectableOnLeft()); }
        else if (v.y > 0) { Debug.Log("Moving to the Up"); SetPlayerSelection(p, playerSelected[p].FindSelectableOnUp()); }
        else if (v.y < 0) { Debug.Log("Moving to the Down"); SetPlayerSelection(p, playerSelected[p].FindSelectableOnDown()); }
                

        // if (v.x == 0) return;

        
        // Debug.Log("PLAYER ACTUALLY MOVED " + players[p]);

        // if (v.x < 0)
        // {
        //     for (int i = playerSelectedButton[p] + 1; i < playerSelectedButton[p] + buttons.Length; i++)
        //     {
        //         int j = i % buttons.Length;
        //         if (!buttons[j].gameObject.activeInHierarchy) continue;

        //         //cursors[playerCursor[p]].transform.position = buttons[j].transform.position;
        //         playerSelectedButton[p] = j;
        //         SetCursorLocation(cursors[playerCursor[p]], buttons[j]);
        //         Debug.Log("Player moved select to " + j);
        //         break;
        //     }
        // }
        // else
        // {
        //     for (int i = playerSelectedButton[p] + buttons.Length - 1; i > playerSelectedButton[p]; i--)
        //     {
        //         int j = i % buttons.Length;
        //         if (!buttons[j].gameObject.activeInHierarchy) continue;

        //         playerSelectedButton[p] = j;
        //         //cursors[playerCursor[p]].transform.position = buttons[j].transform.position;
        //         SetCursorLocation(cursors[playerCursor[p]], buttons[j]);
        //         Debug.Log("Player moved select to " + j);
        //         break;
        //     }
        // }
    }

    private void SetPlayerSelection(Player p, Selectable s)
    {
        Debug.Log("setting player selection to " + s);
        lastSelected = s;
        if (s == null) return;
        Debug.Log("ok, but actually setting player selection to " + s);

        playerSelected[p] = s;
        SetCursorLocation(cursors[playerCursor[p]], s);
    }

    private void SetCursorLocation(GameObject g, Selectable b)
    {
        g.transform.SetParent(b.transform);
        g.GetComponent<RectTransform>().anchoredPosition = new Vector2();
    }

    public bool CheckNewPlayer(Player p)
    {
        if (!players.ContainsKey(p))
        {
            int playerNum;
            for(playerNum = 0; playerNum < GameState.players.Length; playerNum++)
            {
                if (GameState.players[playerNum] == p) break;
            }

            if (playerNum >= GameState.players.Length) return false;

            players[p] = playerNum;
            //playerSelectedButton[p] = 0;
            playerCursor[p] = playerNum;
            playerSelected[p] = FindFirst(); //GameObject.FindObjectOfType<Selectable>();//buttons[0];
            allPlayers.Add(p);

            Debug.Log("player selected initialized to " + playerSelected[p]);

            cursors[playerNum].SetActive(true);
        }

        return true;
    }
}
