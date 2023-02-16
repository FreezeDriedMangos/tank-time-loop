using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectorManager : MonoBehaviour
{
    public Text p1display;
    public Text p2display;
    public Text p3display;
    public Text p4display;
    
    public Text p1aiTypedisplay;
    public Text p2aiTypedisplay;
    public Text p3aiTypedisplay;
    public Text p4aiTypedisplay;
    


    public GameObject humanP1;
    public GameObject humanP2;
    public GameObject humanP3;
    public GameObject humanP4;
    
    public GameObject[] aiP1s;
    public GameObject[] aiP2s;
    public GameObject[] aiP3s;
    public GameObject[] aiP4s;

    //public GameObject[] selected;
    public bool[] playerIsComputer;


    [SerializeField] private int[] enemyTypeSelected;
    [SerializeField] private bool[] playerHasJoined;
    private bool[] playerLocked = new bool[4];
    
    public GameObject[] hiders;
    public GameObject[] aiTypeSelectors;
    public Text[] inputMethodDisplay;

    public ControlsDisplay[] controlDisplays;

    public GameObject[] hiddeners;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] GetSelected()
    {
        GameObject[] humanTanks = new GameObject[4] {humanP1, humanP2, humanP3, humanP4};
        GameObject[][] aiTanks = new GameObject[4][] {aiP1s, aiP2s, aiP3s, aiP4s};
        

        GameObject[] g = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            if (!playerHasJoined[i])
            {
                g[i] = null;
                continue;
            }

            if (!playerIsComputer[i])
                g[i] = humanTanks[i];
            else
                g[i] = aiTanks[i][enemyTypeSelected[i]];
        }

        return g;
    }

    public bool[] GetPlayersAreComputers()
    {
        if (playerIsComputer.Length != 4)
        {
            Debug.LogError("Error with PlayerSelectorManager! playerIsComputer array length was " + playerIsComputer.Length);
            return new bool[4] {false, false, false, false};
        }

        return playerIsComputer;
    }
    
    public void SelectRandom(int playerNum)
    {
        Debug.Log("random");
        GameObject[] enemyTypes = null;
        Text typeDisplay = null;

        switch (playerNum)
        {
            case 0: enemyTypes = aiP1s; typeDisplay = p1aiTypedisplay; break;
            case 1: enemyTypes = aiP2s; typeDisplay = p2aiTypedisplay; break;
            case 2: enemyTypes = aiP3s; typeDisplay = p3aiTypedisplay; break;
            case 3: enemyTypes = aiP4s; typeDisplay = p4aiTypedisplay; break;
            
            default: return;
        }

        Debug.Log("rolling, " + enemyTypes.Length);

        enemyTypeSelected[playerNum] = Random.Range(0, enemyTypes.Length) % enemyTypes.Length;
        string str = enemyTypes[enemyTypeSelected[playerNum]].GetComponent<TankController>().type.ToString();
        typeDisplay.text = string.Join(" ", str.Split('_'));
    }

    public void IncrementSelectedType(int playerNum)
    {
        ChangeSelectedType(playerNum, 1);
    }

    public void DecrementSelectedType(int playerNum)
    {
        ChangeSelectedType(playerNum, -1);
    }

    public void ChangeSelectedType(int playerNum, int delta)
    {
        Debug.Log("setting ai type for  " + playerNum);

        GameObject[] enemyTypes = null;
        Text typeDisplay = null;

        switch (playerNum)
        {
            case 0: enemyTypes = aiP1s; typeDisplay = p1aiTypedisplay; break;
            case 1: enemyTypes = aiP2s; typeDisplay = p2aiTypedisplay; break;
            case 2: enemyTypes = aiP3s; typeDisplay = p3aiTypedisplay; break;
            case 3: enemyTypes = aiP4s; typeDisplay = p4aiTypedisplay; break;
            
            default: return;
        }

        int v = enemyTypeSelected[playerNum] + delta;
        int max = enemyTypes.Length;

        enemyTypeSelected[playerNum] = ((v % max) + max) % max;
        string str = enemyTypes[enemyTypeSelected[playerNum]].GetComponent<TankController>().type.ToString();
        typeDisplay.text = string.Join(" ", str.Split('_'));
    }

    public void SetComputer(int playerNum, bool isComputer)
    {
        if (playerIsComputer[playerNum] != isComputer) 
            ToggleHumanComputer(playerNum);
    }

    public void SetPlayerInputTypeName(int playerNum, string inputName)
    {
        inputMethodDisplay[playerNum].text = inputName;
        controlDisplays[playerNum].SetDisplay(inputName);
        inputMethodDisplay[playerNum].gameObject.SetActive(true);
    }

    public void ToggleHumanComputer(int playerNum)
    {
        if (playerLocked[playerNum]) return;

        Debug.Log("Toggling " + playerNum);

        bool v = playerIsComputer[playerNum] = !playerIsComputer[playerNum];
        string val = "";
        
        if (v) val = "Computer";
        else val = "Player";

        aiTypeSelectors[playerNum].SetActive(v);
        inputMethodDisplay[playerNum].gameObject.SetActive(!v);

        switch (playerNum)
        {
            case 0: p1display.text = val; break;
            case 1: p2display.text = val; break;
            case 2: p3display.text = val; break;
            case 3: p4display.text = val; break;
            
            default: return;
        }
    }

    public void TogglePlayer(int playerNum)
    {
        playerHasJoined[playerNum] = !playerHasJoined[playerNum];
        hiders[playerNum].SetActive(!playerHasJoined[playerNum]);
        hiddeners[playerNum].SetActive(playerHasJoined[playerNum]);
        Debug.Log("toggling player " + playerNum);
    }

    public void LockPlayer(int i)
    {
        playerLocked[i] = true;
    }

    public void UnlockPlayer(int i)
    {
        playerLocked[i] = false;
    }
}
