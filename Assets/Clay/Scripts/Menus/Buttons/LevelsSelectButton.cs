using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsSelectButton : MonoBehaviour
{
    public Text text;
    public Button button;

    private string worldPath;
    private string worldName;
    public int worldNum;

    public void SetWorld(string a)
    {
        Debug.Log("Being set up for " + a);
        worldPath = a;
        string[] split = a.Split('/');

        text.text = worldName = split[split.Length-1];

        button.onClick.AddListener(Clicked);
        // set button to load arena
    }

    public void SetLocation(int row, int col, int numCols)
    {
        RectTransform rt = GetComponent<RectTransform>();

        float r = -(0.5f + row);
        float c = 0.5f + col - ((float)numCols)/2f;

        rt.anchoredPosition = new Vector2(rt.rect.width*c, rt.rect.height*r);
    }

    public void Clicked()
    {
        FileLoader l = GameObject.FindObjectsOfType<FileLoader>()[0];
        GameState.levelsToBuild = l.LoadLevelsFromWorld(worldPath);
        GameState.levelTheme = l.GetThemeAssetPackForWorld(worldPath);
        GameState.worldName = worldName;

        GameState.currentWorld = worldNum;

        GameObject.FindObjectsOfType<ButtonResponder>()[0].StartLevels();
    }
}
