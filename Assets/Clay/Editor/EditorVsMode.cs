using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(VsGameController))]
public class EditorVsMode : Editor
{
    // VsGameController targ;
    // private bool foldout;

    // List<List<bool>> musicSettingsPerRound;

    // void OnEnable() 
    // {
    //     targ = target as VsGameController;

    //     // because Unity won't serialize Lists, we have to convert back and forth between
    //     // lists and arrays
    //     musicSettingsPerRound = new List<List<bool>>();
    //     for (int i = 0; i < targ.musicSettingsPerRound.Length; i++)
    //         musicSettingsPerRound.Add(new List<bool>(targ.musicSettingsPerRound[i]));
    // }

    // public override void OnInspectorGUI()
    // {



    //     targ.roundLength_seconds   = EditorGUILayout.FloatField("Round Length (seconds)", targ.roundLength_seconds);
    //     targ.roundCooldown_seconds = EditorGUILayout.FloatField("Round Cooldown (seconds)", targ.roundCooldown_seconds);
    //     targ.numRounds             = EditorGUILayout.IntField("Num Rounds", targ.numRounds);

    //     while(targ.numRounds < musicSettingsPerRound.Count)
    //     {
    //         musicSettingsPerRound.RemoveAt(musicSettingsPerRound.Count-1);
    //     }
    //     while(targ.numRounds > musicSettingsPerRound.Count)
    //     {
    //         musicSettingsPerRound.Add(new List<bool>());
    //     }

    //     foldout = EditorGUILayout.Foldout(foldout, "Music Setup");

    //     if (foldout)
    //     {
    //         int numTracks = EditorGUILayout.IntField("Num Tracks", musicSettingsPerRound[0].Count);
    //         for (int i = 0; i < targ.numRounds; i++)
    //         {
    //             GUILayout.BeginHorizontal();
    //             for(int j = 0; j < numTracks; j++)
    //             {
    //                 if (musicSettingsPerRound[i].Count <= j) musicSettingsPerRound[i].Add(false);

    //                 musicSettingsPerRound[i][j] = EditorGUILayout.Toggle(musicSettingsPerRound[i][j]);
    //             }
    //             GUILayout.EndHorizontal();
    //         }
    //     }

    //     bool[][] stupidConversion = new bool[targ.numRounds][];
    //     for (int i = 0; i < targ.numRounds; i++)
    //     {
    //         stupidConversion[i] = new bool[musicSettingsPerRound[0].Count];
    //         for (int j = 0; j < musicSettingsPerRound[i].Count; i++)
    //         {
    //             stupidConversion[i][j] = musicSettingsPerRound[i][j];
    //         }
    //     }

    //     targ.musicSettingsPerRound = stupidConversion;
    // }
}
