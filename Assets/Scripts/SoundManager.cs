using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Files")]
    [SerializeField]
    private SoundFilesAttributes [] sfx;
    [Header("Sound Sliders")]
    [SerializeField]
    private Slider MasterLevel;

    [SerializeField]
    private Slider MusicLevel;

    [SerializeField]
    private Slider SFXLevel;

    private void Awake()
    {   //create the audio source for the effects
        foreach (SoundFilesAttributes effects in sfx)
        {
            effects.source = gameObject.AddComponent<AudioSource>();
            effects.source.clip = effects.clip;
            effects.source.volume = effects.volume;
            effects.source.pitch = effects.pitch;
            effects.source.loop = effects.loop;
            effects.source.playOnAwake = effects.awake;
        }
    }
    private void Start()
    {
        //load previous audio settings
        LoadPrefs();
    }
    
    public void MasterVolumeSlider()
    {
        //save the currrent volume value
        float volume = MasterLevel.value;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        LoadPrefs();
    }
    public void Play(int index)
    {
        sfx[index].source.Play();
    }
    public void MusicSlider()
    {
        //save the current volume value
        float music = MusicLevel.value;
        PlayerPrefs.SetFloat("Music", music);
        //get the level soundtrack
        AudioSource level = gameObject.GetComponent<AudioSource>();
        level.volume = music;//adjust the music level to the slider value
        LoadPrefs();
    }

    public void SFXSlider()
    {
        //save the currrent volume value
        float sfxaudio = SFXLevel.value;//adjust volume for all sfx
        foreach (SoundFilesAttributes effects in sfx)
        {
            effects.source.volume = sfxaudio;
        }
        PlayerPrefs.SetFloat("SFX", sfxaudio);//set new prefs then load it
        LoadPrefs();
    }

    void LoadPrefs()
    {
        //load all volume values
        float master = PlayerPrefs.GetFloat("MasterVolume");
        float sfxaudio = PlayerPrefs.GetFloat("SFX");
        float music = PlayerPrefs.GetFloat("Music");
        MusicLevel.value = music;//adjust music slider
        SFXLevel.value = sfxaudio;//adjust sfx slider
        MasterLevel.value = master;//adjust master slider
        AudioListener.volume = master;//set master volume
    }
}
