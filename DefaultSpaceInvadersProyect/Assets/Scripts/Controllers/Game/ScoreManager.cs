using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event EventHandler<OnScoreChangeArgs> OnScoreChange;

    string LevelLoadedName;

    [field: SerializeField] public int PreviousScore { get; private set; }
    [field: SerializeField] private int InternalScore { get; set; }
    [field: SerializeField] public int ShowScore { get; private set; }

    bool InternalAndShowEquals = false;

    private void Awake()
    {
        Instance = this;
        InternalScore = 0;
        ShowScore = 0;
    }

    private void Start()
    {
        LevelLoadedName = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad")[0].GetComponent<LoadCanonLevelData>().NameSceneToLoad;
        var JsonScore = SaveLoadSystem.LevelScoreLoad(LevelLoadedName);
        PreviousScore = JsonScore != null ? JsonUtility.FromJson<ScoreData2Save>(JsonScore).Score : 0;
    }

    private void Update()
    {
        ShowScore = Mathf.FloorToInt(Mathf.Lerp(ShowScore, InternalScore, 0.1f));
        if (!InternalAndShowEquals)
        {
            OnScoreChange?.Invoke(this, new OnScoreChangeArgs { ActualScore = ShowScore });
        }
        InternalAndShowEquals = InternalScore == ShowScore;
    }

    public void AddScore(int ScoreToGive)
    {
        InternalScore += ScoreToGive;
    }

    public int GetInternalScore()
    {
        return InternalScore;
    }

    public bool IsNewScore()
    {
        return InternalScore > PreviousScore;
    }

    public class OnScoreChangeArgs : EventArgs
    {
        public int ActualScore;
    }

    private void OnDestroy()
    {
        if (InternalScore > PreviousScore)
        {
            string Json = JsonUtility.ToJson(new ScoreData2Save { Score = InternalScore });
            SaveLoadSystem.SaveCanonLevelScore(Json, LevelLoadedName);
        }
    }
}
