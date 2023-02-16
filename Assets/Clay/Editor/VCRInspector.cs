using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VCR))]
public class VCRInspector : Editor
{
    VCR targ;
    int stateNum;

    void OnEnable()
    {
        targ = target as VCR;

        stateNum = StateToInt(targ.state);
    }

    public override void OnInspectorGUI() 
    { 
        string[] states = new string[]
        {
            "IDLE", 
            "RECORDING", 
            "PLAYBACK"
        };
        int selected = EditorGUILayout.Popup("State", stateNum, states); 
        if (stateNum != selected)
        {
            stateNum = selected;
            targ.SetState(IntToState(stateNum));
        }

        EditorGUILayout.LabelField("");

        if(GUILayout.Button("Delete VCR Recording")) 
        {
            stateNum = 0;
            targ.Setup();
        }
    }

    private int StateToInt(VCR.VCRState state)
    {
        switch(state)
        {
            case VCR.VCRState.IDLE:        return 0;
            case VCR.VCRState.RECORDING:   return 1;
            case VCR.VCRState.PLAYINGBACK: return 2;
        }
        return 0;
    }

    private VCR.VCRState IntToState(int i)
    {
        switch(i)
        {
            case 0: return VCR.VCRState.IDLE;
            case 1: return VCR.VCRState.RECORDING;
            case 2: return VCR.VCRState.PLAYINGBACK;
        }
        return VCR.VCRState.IDLE;
    }
}
