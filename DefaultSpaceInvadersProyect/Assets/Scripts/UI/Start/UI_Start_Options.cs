using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start_Options : MonoBehaviour
{
    [SerializeField] private Slider MusicSlider = null;

    public void Awake()
    {
        SetupAllUIElements();
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    private void SetupAllUIElements()
    {
        MusicSlider.onValueChanged.AddListener(OnMusicSliderChange);
    }

    private void OnMusicSliderChange(float Value)
    {
        MenuMusicController.Instance.SetMusicVolumePlayerPrefs(Value);
        MenuMusicController.Instance.SetBGMusicVolume(Value);
    }

    public void ShowOptions()
    {
        InitializeOptions();
        gameObject.SetActive(true);
    }

    public void InitializeOptions()
    {
        MusicSlider.value = MenuMusicController.Instance.GetMusicVolumePlayerPrefs();        
    }    

    public void HideOptions()
    {
        MenuMusicController.Instance.SavePlayerPrefs();
        gameObject.SetActive(false);
    }
}
