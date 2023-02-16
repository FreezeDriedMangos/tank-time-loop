using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private bool setup = false;

    public float fadeInOutSpeed = 1;

    public AudioClip intro;
    public bool      playIntro = true;
    public List<AudioClip> clips;
    public List<AudioClip> clipTrails; // everything that should play at the start of every loop but the first (so it doesn't sound cut off at the loop point)
    public List<bool>      playClip;


    private List<AudioSource> trailSources;
    private List<AudioSource> sources;
    private AudioSource       introSource;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        if(setup) return;
        setup = true;

        introSource = this.gameObject.AddComponent<AudioSource>();
        introSource.clip = intro;
        introSource.playOnAwake = false;

        sources = new List<AudioSource>();//new AudioSource[clips.Count];
        trailSources = new List<AudioSource>();
        for (int i = 0; i < clips.Count; i++) 
        {
            AudioSource a = this.gameObject.AddComponent<AudioSource>();
            a.clip = clips[i];
            a.loop = true;

            //a.mute = !playClip[i];
            sources.Add(a);

            float maxVolume = PlayerPrefs.GetFloat("MusicVolume");
            AudioSource t = null;
            if (clipTrails[i] != null)
            {
                t = this.gameObject.AddComponent<AudioSource>();
                t.clip = clipTrails[i];
                t.loop = true;
                t.volume = maxVolume;

                //t.mute = !playClip[i];
            }
            trailSources.Add(t);
        }

        StartCoroutine("PlayMusic");
    }

    public void RestartMusic()
    {
        StopCoroutine("PlayMusic");

        introSource.Stop();

        for (int i = 0; i < sources.Count; i++) 
        {
            sources[i].Stop();
        }

        for (int i = 0; i < sources.Count; i++) 
        {
            if (trailSources[i] == null) continue;
            trailSources[i].Stop();
        }

        StartCoroutine("PlayMusic");
    }

    // function modified from http://answers.unity.com/answers/904995/view.html
    IEnumerator PlayMusic()
    {
        if (playIntro)
        {
            introSource.Play();
            yield return new WaitForSeconds(intro.length);
        }

        for (int i = 0; i < sources.Count; i++) 
        {
            sources[i].Play();
        }

        yield return new WaitForSeconds(clips[0].length);

        for (int i = 0; i < sources.Count; i++) 
        {
            if (trailSources[i] == null) continue;
            trailSources[i].Play();
        }

        yield return null;
    }

    void Update() 
    {
        float maxVolume = PlayerPrefs.GetFloat("MusicVolume");
        //Debug.Log(maxVolume);

        introSource.volume = maxVolume;

        float targetVolume = 0;
        for (int i = 0; i < clips.Count; i++) 
        {
            if (playClip[i])
                targetVolume = maxVolume; 
            else
                targetVolume = 0;
            
            float volDist = Mathf.Abs(targetVolume - sources[i].volume);
            if (volDist == 0) continue;

            sources[i].volume = Mathf.Lerp(sources[i].volume, targetVolume, Time.deltaTime*fadeInOutSpeed/volDist);
            if (clipTrails[i] != null)
                trailSources[i].volume = Mathf.Lerp(trailSources[i].volume, targetVolume, Time.deltaTime*fadeInOutSpeed/volDist);
        }
    }

    public void UpadatedMusic() 
    {
        // for (int i = 0; i < clips.Count; i++) 
        // {
        //     sources[i].mute = !playClip[i];

        //     if (clipTrails[i] != null)
        //         trailSources[i].mute = !playClip[i];
        // }
    }
}
