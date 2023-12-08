using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ScoreController : MonoBehaviour
{
    [SerializeField] private GameObject NewScoreText;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text MaxScoreText;

    private void OnEnable()
    {
        NewScoreText.SetActive(ScoreManager.Instance.IsNewScore());
        MaxScoreText.gameObject.SetActive(!ScoreManager.Instance.IsNewScore());
        MaxScoreText.SetText("Previous Score: " + ScoreManager.Instance.PreviousScore.ToString());
        ScoreText.SetText("Score: " + ScoreManager.Instance.GetInternalScore().ToString());
    }
}
