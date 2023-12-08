using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private Slider PlayerHPBar;
    [SerializeField] private Slider EnemyHPBar;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private GameObject UIDeathMessage;
    [SerializeField] private GameObject UIWinMessage;

    void Start()
    {
        //PlayerHPBar.maxValue = PlayerController.Instance.Health.HEALTH_MAX;
        //PlayerHPBar.value = PlayerHPBar.maxValue;
        PlayerController.Instance.Health.OnHealthChange += OnPlayerHealthChangeScript;
        PlayerController.Instance.Health.OnDieEvent += PlayerHealth_OnDieEvent;
        ScoreManager.Instance.OnScoreChange += OnScoreChange;
        //EnemyHPBar.maxValue = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossScript>().Health.HEALTH_MAX;
        //EnemyHPBar.value = EnemyHPBar.maxValue;
        BossScript.instance.Health.OnDieEvent += BossHealth_OnDieEvent;
        GameObject.FindGameObjectWithTag("Boss").GetComponent<BossScript>().Health.OnHealthChange += OnBossHealthChange;

    }

    private void BossHealth_OnDieEvent(object sender, System.EventArgs e)
    {
        StartCoroutine(ShowWinMessage());
    }

    IEnumerator ShowWinMessage()
    {
        yield return new WaitForSecondsRealtime(12f);
        UIWinMessage.SetActive(true);
    }

    private void OnScoreChange(object sender, ScoreManager.OnScoreChangeArgs e)
    {
        ScoreText.SetText(e.ActualScore.ToString());
    }

    private void PlayerHealth_OnDieEvent(object sender, System.EventArgs e)
    {
        StartCoroutine(ShowDeathMessage());
    }

    IEnumerator ShowDeathMessage()
    {
        while(Time.timeScale >= 0.3f)
        {
            yield return null;
        }
        UIDeathMessage.SetActive(true);
    }

    public void SetHealthBars()
    {
        PlayerHPBar.maxValue = PlayerController.Instance.Health.HEALTH_MAX;
        PlayerHPBar.value = PlayerHPBar.maxValue;
        EnemyHPBar.maxValue = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossScript>().Health.HEALTH_MAX;
        EnemyHPBar.value = EnemyHPBar.maxValue;
    }

    private void OnBossHealthChange(object sender, HealthClass.OnHealthChangeArgs e)
    {
        EnemyHPBar.value = e.Health.GetCurrentHealth();
    }

    private void OnPlayerHealthChangeScript(object sender, HealthClass.OnHealthChangeArgs e)
    {
        PlayerHPBar.value = e.Health.GetCurrentHealth();
    }

    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

}
