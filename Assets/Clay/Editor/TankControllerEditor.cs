using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TankController))]
public class TankControllerEditor : Editor
{
    TankController targ;

    void OnEnable() 
    {
        targ = target as TankController;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button ("Kill Tank"))
            targ.SetDestroyed(true);

        // Show default inspector property editor
        DrawDefaultInspector ();
    }
}
