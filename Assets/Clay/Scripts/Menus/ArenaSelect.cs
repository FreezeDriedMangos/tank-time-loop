using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaSelect : MonoBehaviour
{
    public Text levelNameText;
    public Text numLevelsText;
    public Text levelNumText;

    public Toggle kothModeToggle;

    public FileLoader f;

    private int levelNumSelected = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelNameText.text = GetNameFor(f.vsMode_arenas[0]);
        numLevelsText.text = ""+ f.vsMode_arenas.Length;
        levelNumText.text = "1";
        
    }

    string GetNameFor(string path)
    {
        string[] split = path.Split('/');
        string[] split2 = split[split.Length-1].Split('\\');

        return split2[split2.Length-1];
    }

    public void IncrementSelected()
    {
        levelNumSelected += 1;
        levelNumSelected = levelNumSelected % f.vsMode_arenas.Length;
        levelNameText.text = GetNameFor(f.vsMode_arenas[levelNumSelected]);
        levelNumText.text = "" + (levelNumSelected+1);
        
    }

    public void DecrementSelected()
    {
        levelNumSelected -= 1;
        levelNumSelected = (levelNumSelected + f.vsMode_arenas.Length) % f.vsMode_arenas.Length;
        levelNameText.text = GetNameFor(f.vsMode_arenas[levelNumSelected]);
        levelNumText.text = "" + (levelNumSelected+1);
    }

    public void SelectRandom()
    {
        levelNumSelected = Random.Range(0, f.vsMode_arenas.Length) % f.vsMode_arenas.Length;
        levelNameText.text = GetNameFor(f.vsMode_arenas[levelNumSelected]);
        levelNumText.text = "" + (levelNumSelected+1);
    }

    public void StartPlaying()
    {
        string arenaPath = f.vsMode_arenas[levelNumSelected];
        string[][] s = {f.LoadArena(arenaPath)};
        //Debug.Log(arenaPath + "\n" + s + "\n" + s[0]);
        GameState.levelsToBuild = s;
        GameState.levelTheme = f.GetThemeAssetPackForArena(arenaPath);
        GameState.levelName = GetNameFor(arenaPath);
        GameState.worldName = "1v1 Arenas";

        //Debug.Log("LEVEL THEME LOADING " + GameState.levelTheme.gameObject.name);

        if (kothModeToggle.isOn)
            GameObject.FindObjectsOfType<ButtonResponder>()[0].StartKotH();
        else
            GameObject.FindObjectsOfType<ButtonResponder>()[0].Start1v1();
    }
}
