using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MasterThemeAssetPack))]
public class MasterThemeAssetPackEditor : Editor
{
    MasterThemeAssetPack targ;

    string currEnter = "";
    ThemeAssetPack currEnterv;

    bool fold = false;

    void OnEnable()
    {
        targ = target as MasterThemeAssetPack;
        Debug.Log(targ + "\n"+target);
    }

    public override void OnInspectorGUI() 
    {
        if (targ == null)
            targ = target as MasterThemeAssetPack;

        GUILayout.BeginHorizontal();
            currEnter = EditorGUILayout.TextField(currEnter);
            currEnterv = EditorGUILayout.ObjectField("", currEnterv, typeof(ThemeAssetPack), true) as ThemeAssetPack;
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Add Theme Asset Pack")) 
        {
            targ.themeNames.Add(currEnter);
            targ.themes.Add(currEnterv);
        }

        fold = EditorGUILayout.Foldout(fold, "Registered Theme Asset Packs");
        if (fold)
            this.ThemesList();
    }

    private void ThemesList() {

        List<int> remove = new List<int>();

        for (int i = 0; i < targ.themeNames.Count; i++)
        {
            GUILayout.BeginHorizontal();
                string k = GUILayout.TextField(targ.themeNames[i]);
                ThemeAssetPack t = EditorGUILayout.ObjectField("", targ.themes[i], typeof(ThemeAssetPack), true) as ThemeAssetPack;
            GUILayout.EndHorizontal();

            if (k == "")
            {
                remove.Add(i);
            }
            
            if (k != targ.themeNames[i])
            {
                targ.themeNames[i] = k;
            }
            
            if (t != targ.themes[i])
            {
                targ.themes[i] = t;
            }
        } 

        for (int i = remove.Count-1; i >= 0; i++)
        {
            targ.themeNames.RemoveAt(remove[i]);
            targ.themes.RemoveAt(remove[i]);
        } 
    }
}
