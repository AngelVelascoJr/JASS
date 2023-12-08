using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    private const string PlayerPrefsNameString = "PlayerName";
    [SerializeField] private GameObject UI_CanonLevelSelect = null;
    [SerializeField] private string PreviewLevel = "";
    //[SerializeField] private string PlayerName = "";

    private void Start()
    {
        SaveLoadSystem.Initialize();
    }

    [Obsolete()]
    public void LoadTestLevels()
    {
        UI_CanonLevelSelect.GetComponent<GetLevelsData>().SetData(PreviewLevel);
    }

    public void LoadLevels()
    {
        GetComponentInChildren<GetLevelsData>().CallAllButtonsToLoad();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
