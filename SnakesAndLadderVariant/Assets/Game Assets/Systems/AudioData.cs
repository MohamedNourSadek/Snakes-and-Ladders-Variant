using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData 
{
    [SerializeField] public Effects name;
    [SerializeField] public AudioClip clip;
    [SerializeField] public float defaultVolume;


    public static AudioData FindClip(List<AudioData> sounds, Effects name)
    {
        return sounds.Find(sound => sound.name == name);
    }
}
