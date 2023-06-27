using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] fxSounds;
    public AudioSource fxSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayFx(string name)
    {
        Sound s = Array.Find(fxSounds, x => x.name == name);
        if (s != null)
        {
            fxSource.PlayOneShot(s.clip);
        }
    }
}
