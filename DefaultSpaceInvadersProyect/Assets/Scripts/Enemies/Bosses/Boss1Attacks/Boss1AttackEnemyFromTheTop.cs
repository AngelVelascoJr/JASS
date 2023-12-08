using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Boss1AttackEnemyFromTheTop : BossAttackBase
{

    public List<GameObject> EnemyToSpawn;
    [SerializeField] private GameObject UIWarning;
    private List<WaitAndRunEnemyScript> spawned;

    private float TimeToAttack;
    float counter = 0;
    private int EnemyMax;
    public float Radius = 1.5f;

    [SerializeField] private float EnemyVelocity;

    protected override void Awake()
    {
        gameObject.transform.position = PlayerController.Instance.transform.position;
        TimeToAttack = Random.Range(1.2f, 2f);
        EnemyMax = Random.Range(10, 20);
    }

    protected override void Start()
    {
        AttackType = AttackTypeEnum.FrenessyAttackFromTop;
        PrepareDataToAttack();
    }

    protected override void Update()
    {
        counter += Time.deltaTime;
    }

    protected override void PrepareDataToAttack()
    {
        spawned = new List<WaitAndRunEnemyScript>();
        UIWarning.transform.position = new Vector3(UIWarning.transform.position.x, UIWarning.transform.position.y, 0f);
        StartCoroutine(SpawnEnemyCorroutine());
    }

    public override void KillAll()
    {
        foreach(var enemy in spawned)
        {
            IDamageable EnemyToDamage;
            if(TryGetComponent(out EnemyToDamage))
            {
                var HealthClass = EnemyToDamage.GetHealthClass();
                HealthClass.WasHitted(HealthClass.HEALTH_MAX);
            }
            else
            {
                Destroy(enemy);
            }
        }
    }

    IEnumerator SpawnEnemyCorroutine()
    {
        for (int i = 0; i < EnemyMax; i++)
        {
            var enemySpawned = Instantiate(EnemyToSpawn[Random.Range(0, EnemyToSpawn.Count)]);
            Vector3 StayPos = new Vector3(gameObject.transform.position.x + Random.Range(-Radius, Radius), 0, i + EnemySpawnPositions.Instance.SpawnPoints[2].z);
            var comp = enemySpawned.GetComponent<WaitAndRunEnemyScript>();
            spawned.Add(comp);
            enemySpawned.name = enemySpawned.name + StayPos.x + "," + StayPos.z;
            enemySpawned.transform.position = StayPos;
            comp.Velocity = EnemyVelocity;
            comp.Direction = -transform.forward;
            comp.OnEnemyDieEvent += Comp_OnEnemyDieEvent;
            yield return null;
        }
        ActivateAttack();
    }

    private void Comp_OnEnemyDieEvent(object sender, System.EventArgs e)
    {
        spawned.Remove(((WaitAndRunEnemyScript)sender));
    }

    protected override void ActivateAttack()
    {
        StartCoroutine(Warning());
        StartCoroutine(EnemyAttackCorroutine());
    }

    IEnumerator Warning()
    {
        while(counter <= TimeToAttack)
        {
            yield return new WaitForSeconds(0.25f);
            UIWarning.SetActive(!UIWarning.activeInHierarchy);
        }
        UIWarning.SetActive(false);
    }

    IEnumerator EnemyAttackCorroutine()
    {
        yield return new WaitForSeconds(TimeToAttack);
        foreach (WaitAndRunEnemyScript enemy in spawned)
        {
            enemy.GO = true;
        }
        while (spawned.Count >= 0)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Boss1AttackEnemyFromTheTop))]
public class WaitAndRunManagerEditor : Editor
{
    void OnSceneGUI()
    {
        Boss1AttackEnemyFromTheTop myEnemyScriptRef = target as Boss1AttackEnemyFromTheTop;
        var tr = myEnemyScriptRef.transform;

        var colorOrange = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = colorOrange;

        EditorGUI.BeginChangeCheck();

        Handles.DrawLine(tr.position - new Vector3(myEnemyScriptRef.Radius, 0), tr.position + new Vector3(myEnemyScriptRef.Radius, 0));

        EditorGUI.EndChangeCheck();
    }
}

#endif