using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterThemeAssetPack : MonoBehaviour
{
    Dictionary<string, ThemeAssetPack> masterList;

    public List<string> themeNames = new List<string>();
    public List<ThemeAssetPack> themes = new List<ThemeAssetPack>();
    

    // Start is called before the first frame update
    void Start()
    {
        masterList = new Dictionary<string, ThemeAssetPack>();
        
        for (int i = 0; i < themeNames.Count; i++)
        {
            //Debug.Log("THEME *" + themeNames[i] + "* = *" + themes[i].gameObject.name + "*");
            masterList[themeNames[i]] = themes[i];
        }

        ThemeAssetPack.masterList = this.masterList;


    }
}
