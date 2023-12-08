using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : OptionsInherited
{
    public static MusicController Instance { get; private set; }

    [SerializeField] private List<AudioSource> AudioSources;
    [SerializeField] private List<AudioClip> Audios;

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private float WFSecs = 3f;
    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        ResetPitchVolume();
        SetBGMusicVolume(GetMusicVolumePlayerPrefs());
        StartCoroutine(PlayBGMusic());
        PlayerController.Instance.Health.OnHealthChange += Player_OnHealthChange;
        PlayerController.Instance.Health.OnDieEvent += PlayerHealth_OnDieEvent;
    }

    private void PlayerHealth_OnDieEvent(object sender, System.EventArgs e)
    {
        StartCoroutine(SlowAllMusic());
    }

    IEnumerator SlowAllMusic()
    {
        yield return new WaitForSeconds(1.3f);
        var pitch = 100f;
        audioMixer.GetFloat("MasterPitch", out pitch);
        while (pitch > 0.1f)
        {
            audioMixer.SetFloat("MasterPitch", pitch - Time.deltaTime);
            yield return null;
            audioMixer.GetFloat("MasterPitch", out pitch);
        }
    }

    private void Player_OnHealthChange(object sender, HealthClass.OnHealthChangeArgs e)
    {
        if (e.Health.GetCurrentHealthNormalized() < 0.3f && !AudioSources[1].isPlaying)
        {
            PlaySound(1, 2);
        }
        else if(e.Health.GetCurrentHealthNormalized() >= 0.3f)
        {
            StopSound(1);
        }
    }

    private IEnumerator PlayBGMusic()
    {
        yield return new WaitForSeconds(WFSecs);
        PlaySound(0, 1);
        SetLoop(0, true);
    }

    public void PlaySound(int AudioSourceNum, int AudioNum)
    {
        AudioSources[AudioSourceNum].clip = Audios[AudioNum];
        AudioSources[AudioSourceNum].Play();
    }

    public void StopSound(int AudioSourceNum)
    {
        AudioSources[AudioSourceNum].Stop();
    }

    public void SetLoop(int AudioSourceNum, bool loop)
    {
        AudioSources[AudioSourceNum].loop = loop;
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
