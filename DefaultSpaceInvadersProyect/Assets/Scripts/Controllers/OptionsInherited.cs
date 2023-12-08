using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsInherited : MonoBehaviour
{
    public const string MasterVolumePlayerPref = "MasterVolume";
    public const string MusicVolumePlayerPref = "MusicVolume";
    public const string VFXVolumePlayerPref = "MusicVolume";

    public void SetMasterVolumePlayerPref(float NewVolume)
    {
        PlayerPrefs.SetFloat(MasterVolumePlayerPref, NewVolume);
        Debug.Log("Saved Master volume as: " + NewVolume.ToString());
    }

    public void SetMusicVolumePlayerPrefs(float NewVolume)
    {
        PlayerPrefs.SetFloat(MusicVolumePlayerPref, NewVolume);
        Debug.Log("Saved Music volume as: " + NewVolume.ToString());
    }

    public void SetVFXVolumePlayerPrefs(float NewVolume)
    {
        PlayerPrefs.SetFloat(VFXVolumePlayerPref, NewVolume);
        Debug.Log("Saved VFX volume as: " + NewVolume.ToString());
    }

    public float GetMasterVolumePlayerPref()
    {
        return PlayerPrefs.GetFloat(MasterVolumePlayerPref, 1);
    }

    public float GetMusicVolumePlayerPrefs()
    {
        Debug.Log("initialized music volume as: " + PlayerPrefs.GetFloat(MusicVolumePlayerPref));
        return PlayerPrefs.GetFloat(MusicVolumePlayerPref, 1);
    }

    public float GetVFXVolumePlayerPrefs()
    {
        return PlayerPrefs.GetFloat(VFXVolumePlayerPref, 1);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }

}
