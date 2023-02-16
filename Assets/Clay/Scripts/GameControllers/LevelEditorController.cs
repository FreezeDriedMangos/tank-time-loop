using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorController : MonoBehaviour
{
    public CursorController[] cursors;
    public LevelBuilder levelBuilder;

    public List<List<List<string>>> level;

    public int width = 10;
    public int height = 7;
    public int numFloors = 2;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!GameState.playerJoined[i]) continue;
            cursors[i] = GameState.players[i].cursor = GameObject.Instantiate(cursors[i]);
            
            InputHandler h = new InputHandler();
            h.pNum = i;
            h.lec = this;

            GameState.players[i].onFireNotify = h.LeftClick;
            GameState.players[i].onShootRocketNotify = h.RightClick;
        }

        //levelBuilder.BuildFloor();

        ResizeLevel();

        GameState.levelsToBuild[0] = FormatLevelForExport();

        levelBuilder.BuildLevel();
    }

    void ResizeLevel()
    {
        List<List<List<string>>> oldLevel = level;

        for (int i = 0; i < numFloors; i++)
        {
            level[i] = new List<List<string>>();
            for (int j = 0; j < width; j++)
            {
                level[i][j] = new List<string>();
                for (int k = 0; k < height; k++)
                {
                    level[i][j][k] = ".";
                }
            }
        }

        if (oldLevel == null) return;

        int f = Mathf.Min(numFloors, oldLevel.Count);
        int w = Mathf.Min(width, oldLevel[0].Count);
        int h = Mathf.Min(height, oldLevel[0][0].Count);
        for (int i = 0; i < f; i++)
        {
            for (int j = 0; j < w; j++)
            {
                for (int k = 0; k < h; k++)
                {
                    level[i][j][k] = oldLevel[i][j][k];
                }
            }
        }
    }

    string[] FormatLevelForExport()
    {
        string[] exp = new string[numFloors];

        for (int i = 0; i < numFloors; i++)
        {
            string floorString = "";
            for (int j = 0; j < width; j++)
            {
                floorString += string.Join(",", level[i][j]) + "\n";
            }
            exp[i] = floorString;
        }

        return exp;
    }

    public void LeftClick(int playerNum) {}
    public void RightClick(int playerNum) {}
    public void MiddleClick(int playerNum) {}
}

class InputHandler 
{
    public int pNum;
    public LevelEditorController lec;

    public void LeftClick()
    {
        lec.LeftClick(pNum);
    }

    public void RightClick()
    {
        lec.RightClick(pNum);
    }
}
