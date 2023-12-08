using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ChangeScenes))]
public class GetLevelsData : MonoBehaviour
{

    private LoadCanonLevelData _CanonLevelData = null;
    [SerializeField] private TMP_Text TextDisplay;
    [SerializeField] private string LevelName = "";


    void Start()
    {
        _CanonLevelData = GameObject.FindGameObjectWithTag("DontDestroyOnLoad").GetComponent<LoadCanonLevelData>();
    }

    public void SetData(string name)
    {
        _CanonLevelData.NameToChangeScene(name);
        gameObject.GetComponent<ChangeScenes>().ChangeScene("GameScene");
    }

    public void CallAllButtonsToLoad()
    {
        var LevelFiles = System.IO.Directory.GetFiles(SaveLoadSystem.LEVELS_PATH, "*?" + SaveLoadSystem.EXTENSION);
        Debug.Log("founded " + LevelFiles.Length + " levels");
        for (int i = 0; i < LevelFiles.Length; i++)
        {
            if (SaveLoadSystem.LevelExists(LevelName) && System.IO.Path.GetFileNameWithoutExtension(LevelFiles[i]) == LevelName)
            {
                string StringLevelData = SaveLoadSystem.LevelLoad(LevelName);
                string StringLevelScoreData = SaveLoadSystem.LevelScoreLoad(LevelName);
                LevelData2Save JsonLevelData = JsonUtility.FromJson<LevelData2Save>(StringLevelData);
                if (StringLevelScoreData == null)
                {
                    JsonLevelData.Score = 0;
                }
                else
                {
                    JsonLevelData.Score = JsonUtility.FromJson<ScoreData2Save>(StringLevelScoreData).Score;
                }

                TextDisplay.SetText(JsonLevelData.Name + " \tScore: " + JsonLevelData.Score.ToString());
                
                var button = GetComponent<Button>();                
                button.onClick.AddListener(() => OnClickFunc(LevelName));
                return;
            }
        }
        Debug.Log("Level Not Founded: " + LevelName + ", in " + LevelFiles);
    }

    private void OnClickFunc(string LevelName)
    {
        SetData(LevelName);
    }
}
