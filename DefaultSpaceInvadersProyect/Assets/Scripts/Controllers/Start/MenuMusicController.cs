using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class MenuMusicController : OptionsInherited
{
    public static MenuMusicController Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        ResetPitchVolume();
        SetBGMusicVolume(GetMusicVolumePlayerPrefs());
    }

    public void SetBGMusicVolume(float vol)
    {
        audioMixer.SetFloat("BGMVol", vol);
    }

    private void ResetPitchVolume()
    {
        audioMixer.SetFloat("MasterPitch", 1f);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ResetPitchVolume();
    }
}
