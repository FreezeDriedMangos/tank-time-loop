using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchReactor))]
public class SwitchReactorEditor : Editor
{
    SwitchReactor targ;

    void OnEnable()
    {
        targ = target as SwitchReactor;
    }

    public override void OnInspectorGUI() 
    {
        if (targ == null)
            targ = target as SwitchReactor;

        if(GUILayout.Button("Force Trigger Switch")) 
        {
            targ.SwitchEnabled();
        }

        if(GUILayout.Button("Force Untrigger Switch")) 
        {
            targ.SwitchDisabled();
        }
    }
}
