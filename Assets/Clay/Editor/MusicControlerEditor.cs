using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicController))]
public class MusicControlerEditor : Editor
{
    MusicController targ;
    private bool numLayersUnprotected = false;

    [SerializeField] int numClips;

    void OnEnable()
    {
        targ = target as MusicController;
        
        // clips = targ.clips;
        numClips = targ.clips.Count;

        // intro = targ.intro;
        // playIntro = targ.playIntro;

        // while (targ.playClip.Count < numClips) targ.playClip.Add(false);
        // enabled = targ.playClip;

    }

    public override void OnInspectorGUI() 
    {        
        targ.fadeInOutSpeed = EditorGUILayout.FloatField("Fade In/Out Speed", targ.fadeInOutSpeed);

        GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Intro Clip: ");
            targ.playIntro = EditorGUILayout.Toggle(targ.playIntro);
            targ.intro = EditorGUILayout.ObjectField("", targ.intro, typeof(AudioClip), true) as AudioClip;
        GUILayout.EndHorizontal();

        // EditorGUILayout.LabelField("Track Clips: ");

        numLayersUnprotected = EditorGUILayout.Foldout(numLayersUnprotected, "Change Number of Layers");
        if (numLayersUnprotected)
        {
            int numClipsTemp = EditorGUILayout.IntField("Number of Layers: ", numClips);

            while (numClipsTemp < numClips) 
            {
                int n = targ.clips.Count;
                targ.clips.RemoveAt(n-1);
                targ.playClip.RemoveAt(n-1);
                targ.clipTrails.RemoveAt(n-1);
                numClips--;
            }
            while (numClipsTemp > numClips) 
            {
                targ.clips.Add(null);
                targ.clipTrails.Add(null);
                targ.playClip.Add(false);
                numClips++;
            }
        }

        EditorGUILayout.LabelField("Enabled\t\tLayer Clip\t\tLayer's Trail Clip");
        for (int i = 0; i < numClips; i++)
        {
            GUILayout.BeginHorizontal();
                targ.playClip[i] = EditorGUILayout.Toggle(targ.playClip[i]);
                targ.clips[i] = EditorGUILayout.ObjectField("", targ.clips[i], typeof(AudioClip), true) as AudioClip;
                targ.clipTrails[i] = EditorGUILayout.ObjectField("", targ.clipTrails[i], typeof(AudioClip), true) as AudioClip;

                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        targ.UpadatedMusic();

        // foldedTails = EditorGUILayout.Foldout(foldedTails, "Tails");
        // if (foldedTails)
        // {
        //     for (int i = 0; i < numClips; i++)
        //     {
        //         targ.clipTrails[i] = EditorGUILayout.ObjectField("", targ.clipTrails[i], typeof(AudioClip), true) as AudioClip;
        //         // GUILayout.BeginHorizontal();
        //         //     targ.playClip[i] = EditorGUILayout.Toggle(targ.playClip[i]);
        //         //     targ.clips[i] = EditorGUILayout.ObjectField("", targ.clips[i], typeof(AudioClip), true) as AudioClip;
                
        //         //     GUILayout.FlexibleSpace();
        //         // GUILayout.EndHorizontal();
        //     }
        // }
    }
}
