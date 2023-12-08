using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance { get; private set; }
    public float UniversalTime { get; private set; }

    public bool OnPlayMode { get; private set; }

    private void Awake()
    {
        Instance = this;
        UniversalTime = 0;
        OnPlayMode = false;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        PlayerController.Instance.Health.OnDieEvent += PlayerHealth_OnDieEvent;
    }

    private void PlayerHealth_OnDieEvent(object sender, System.EventArgs e)
    {
        StartCoroutine(SlowTime());
    }

    IEnumerator SlowTime()
    {
        yield return new WaitForSeconds(1.5f);
        while(Time.timeScale > 0.1f)
        {
            Time.timeScale -= Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UniversalTime += Time.deltaTime;
    }

    private void ResetUniversalTime()
    {
        UniversalTime = 0;
    }

    public void SetPlayMode(bool set)
    {
        OnPlayMode = set;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
    }

}
