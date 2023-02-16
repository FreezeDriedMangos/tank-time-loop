using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaSelectButton : MonoBehaviour
{
    public Text text;
    public Button button;

    private string arenaPath;
    private string arenaName;

    public void SetArena(string a)
    {
        //Debug.Log("Being set up for " + a);
        arenaPath = a;
        string[] split = a.Split('/');
        split = split[split.Length-1].Split('\\');

        text.text = arenaName = split[split.Length-1];
        button.onClick.AddListener(Clicked);
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
        string[][] s = {l.LoadArena(arenaPath)};
        //Debug.Log(arenaPath + "\n" + s + "\n" + s[0]);
        GameState.levelsToBuild = s;
        GameState.levelTheme = l.GetThemeAssetPackForArena(arenaPath);
        GameState.levelName = arenaName;
        GameState.worldName = "1v1 Arenas";

        //Debug.Log("LEVEL THEME LOADING " + GameState.levelTheme.gameObject.name);

        GameObject.FindObjectsOfType<ButtonResponder>()[0].Start1v1();
    }
}
