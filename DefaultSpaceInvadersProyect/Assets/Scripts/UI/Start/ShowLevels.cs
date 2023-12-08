using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLevels : MonoBehaviour
{
    [SerializeField] private GameObject UILevelPrefab = null;
    [SerializeField] private GameObject InvisibleContainer = null;
    [SerializeField] List<GameObject> GOs;

    public enum GameScenesToLoad
    {
        Game,
        ToEditScene
    }

    private void Awake()
    {
        GOs = new List<GameObject>(0);
    }

    public void SetUILevels()
    {
        if(GOs.Count != 0)
        {
            foreach(GameObject GO in GOs)
            {
                Destroy(GO);
            }
        }
        //StartCoroutine(ShowLevelsCoroutine());
    }

    //private IEnumerator ShowLevelsCoroutine()
    //{
        //MyNameSpace.Utils.CoroutineWithData cd = new MyNameSpace.Utils.CoroutineWithData(this, DataClassification.Instance.GetLevelDatasAsLevelData());
        //yield return cd.Coroutine;
        //LevelData[] LevelDatas = (LevelData[])cd.result;
        //Debug.Log("levelQuantity: " + LevelDatas.Length.ToString());
        //if (LevelDatas.Length != 0)
        //{
        //    foreach (LevelData data in LevelDatas)
        //    {
        //        InstantiateUILevel(data, GameScenesToLoad.Game);
        //    }
        //}
        //else
        //{
        //    Debug.Log("No Levels Found");
        //} 
    //}

    private void InstantiateUILevel(/*LevelData data,*/ GameScenesToLoad SceneToLoad)
    {
        GameObject GO = Instantiate(UILevelPrefab, InvisibleContainer.transform);
        UniversalTag[] Texts = GO.GetComponentsInChildren<UniversalTag>();
        //ScoreData scoreData = DataClassification.LoadScore(data.Name);
        foreach (UniversalTag text in Texts)
        {
            if (text.Index == 0)
            {
                //text.GetComponent<TMP_Text>().text = data.Name;
            }
            else if (text.Index == 1)
            {
                //if (scoreData == null)
                    text.GetComponent<TMP_Text>().text = "Score: 0";
                //else
                {
                    //text.GetComponent<TMP_Text>().text = "InternalScore: " + scoreData.InternalScore.ToString();
                    //data.MaxScore = scoreData.InternalScore;
                }
            }
        }
        GO.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            //StartLevel(data, SceneToLoad);
        });
        GOs.Add(GO);
    }

    public void StartLevel(/*LevelData levelData,*/ GameScenesToLoad SceneToLoad)
    {
        //DataContainer.Instance.SetLevelData(levelData);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(SceneToLoad.ToString());
    }
}
