using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{/*
    [Header("EventListener")]
    public playAudioEventSO FXEvent;
    public playAudioEventSO BGMEvent;
    public FloatEventSO VolumeChangeEvent;
    public VoidEventSO PauseEvent;

    [Header("¹ã²¥")]
    public FloatEventSO syncVolumeEvent;*/


    [Header("AudioSrouce")]
    public AudioSource BGMSource;
    public AudioSource SFXSource;
    public AudioMixer mixer;

    private void OnEnable()
    {

    }




    private void OnDisable()
    {

    }

    private void OnPauseEvent()
    {
        float amount;
        mixer.GetFloat("MasterVolume", out amount);
    }

    private void OnVolumeChangeEvent(float amount)
    {
        amount = amount * 100 - 80;
        mixer.SetFloat("MasterVolume",amount);
    }
    private void OnFxEvent(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }
    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }
}

