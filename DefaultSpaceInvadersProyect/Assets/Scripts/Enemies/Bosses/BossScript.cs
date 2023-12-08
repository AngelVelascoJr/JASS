using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

public class BossScript : MonoBehaviour, IDamageable
{
    public static BossScript instance { get; private set; }
    [field: SerializeField] public HealthClass Health { get; private set; }
    public Gradient HealthGradient;
    [SerializeField] private int MaxHealth = 1000;

    [SerializeField] private GameObject DieParticleEfect;

    [SerializeField] private float AttackValue;
    [SerializeField] private List<int> AttackValueList;
    [SerializeField] private List<GameObject> BossAttackListGO;
    [SerializeField] private List<GameObject> BossSpawnedAttacksList;

    [SerializeField] private List<GameObject> BossAttacksToSpawnOnStart;

    private float TimerForAttack = 0;
    [SerializeField] private float TimerForAttackMax = 5f;

    [SerializeField] private CinemachineVirtualCamera VCam;
    [SerializeField] private int Score;
    private void Awake()
    {
        instance = this;
        Health = new HealthClass(MaxHealth);
        if (AttackValueList.Count != BossAttackListGO.Count)
        {
            Debug.LogWarning("AttacklistCount is not equal to AttackValueListCount, some attacks will be ignored");
        }
        AttackValue = Random.Range(AttackValueList.Min(), AttackValueList.Max() + 5);
        TimerForAttack = 0f;
        Health.OnDieEvent += OnDieEvent;
    }

    private void OnDieEvent(object sender, System.EventArgs e)
    {
        //Time.timeScale = 0f;
        StartCoroutine(OnDie());
    }

    IEnumerator OnDie()
    {
        CinemachineBasicMultiChannelPerlin Noice = VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Noice.m_AmplitudeGain = 0.5f;
        yield return new WaitForSecondsRealtime(0.5f);
        Noice.m_AmplitudeGain = 0;
        yield return new WaitForSecondsRealtime(1.2f);
        Noice.m_AmplitudeGain = 0.5f;
        yield return new WaitForSecondsRealtime(0.5f);
        Noice.m_AmplitudeGain = 0; 
        yield return new WaitForSecondsRealtime(1.2f);
        while (Noice.m_AmplitudeGain < 0.8f)
        {
            Debug.Log(Time.deltaTime * (2 / 3));
            Noice.m_AmplitudeGain += Time.deltaTime * (2f / 3f);
            yield return null;
        }
        Instantiate(DieParticleEfect, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        yield return new WaitForSecondsRealtime(2.0f);
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        ScoreManager.Instance.AddScore(Score);
        yield return new WaitForSecondsRealtime(0.5f);
        Noice.m_AmplitudeGain = 0f;
        foreach(var go in BossSpawnedAttacksList)
        {
            go.GetComponent<BossAttackBase>().KillAll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerForAttack > TimerForAttackMax && !Health.IsDead())
        {
            TimerForAttack = 0;
            var index = GetInmediateAttackIndex();
            SpawnAttack(index);
        }
        TimerForAttack += Time.deltaTime;
    }

    public void SpawnInitialAttacks()
    {
        StartCoroutine(WaitSomeSecs(1f));
    }

    IEnumerator WaitSomeSecs(float Secs)
    {
        yield return new WaitForSeconds(Secs);
        SpawnInitialAttackAfterSecs();
    }

    private void SpawnInitialAttackAfterSecs()
    {
        foreach (GameObject Attack in BossAttacksToSpawnOnStart)
        {
            var attack = Instantiate(Attack);
            BossSpawnedAttacksList.Add(attack);
        }
    }

    private int GetInmediateAttackIndex()
    {
        int AttackIndex = 0;
        if(AttackValue < AttackValueList.Min()) AttackValue = Random.Range(AttackValueList.Min(), AttackValueList.Max() + 5);
        foreach (int AttackValueInList in AttackValueList)
        {
            if (AttackValueInList < AttackValue)
            {
                AttackIndex = AttackValueList.IndexOf(AttackValueInList);
            }
            else
            {
                break;
            }
        }
        AttackValue -= Random.Range(1, 5);
        return AttackIndex;
    }

    protected virtual void SpawnAttack(int index)
    {
        var attack = Instantiate(BossAttackListGO[index]);
        BossSpawnedAttacksList.Add(attack);
    }

    public HealthClass GetHealthClass()
    {
        return Health;
    }
}
