using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum Effects { ButtonPress, Roll, Error, Fail, Successs}

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip globalMusic;
    [SerializeField] float globalMusicVolume;

    [Header("Effects")]
    [SerializeField] List<AudioData> effects = new List<AudioData>();

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        this.transform.parent = null;
        DontDestroyOnLoad(this);
        PlayMusic();
    }
    public void PlayMusic()
    {
        AudioSource music = this.AddComponent<AudioSource>();
        music.clip = globalMusic;
        music.volume = globalMusicVolume;
        music.loop = true;
        music.Play();
    }
    public void PlayEffect(Effects effectName)
    {
        AudioData effect = effects.Find(sound => sound.name == effectName);
        AudioSource.PlayClipAtPoint(effect.clip, Camera.main.transform.position + (Camera.main.transform.forward * 1), effect.defaultVolume);
    }
}
