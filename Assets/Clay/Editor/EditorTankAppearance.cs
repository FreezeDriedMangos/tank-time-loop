using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TankAppearance))]
public class EditorTankAppearance : Editor
{
    TankAppearance targ;

    void OnEnable() 
    {
        targ = target as TankAppearance;
        targ.Setup();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button ("Natural Appearance"))
            targ.SetNatural();
        if (GUILayout.Button ("Hologram Appearance"))
            targ.SetHologram(0);
        if (GUILayout.Button ("Rogue Appearance"))
            targ.SetRogue(0);

        // Show default inspector property editor
        DrawDefaultInspector ();
    }
}