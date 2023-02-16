using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelsGameController))]
public class LevelsModeControllerEditor : Editor
{
    LevelsGameController targ;

    void OnEnable() 
    {
        targ = target as LevelsGameController;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button ("Force Next Level"))
        {
            foreach (TankController tc in GameObject.FindObjectsOfType<TankController>())
            {
                tc.SetDestroyed(true);
            }
        }

        // Show default inspector property editor
        DrawDefaultInspector ();
    }
}
