using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class Boss1AttackSpawnEnemy : BossAttackBase
{

    public List<GameObject> EnemyToSpawn;
    private List<GameObject> spawned;
    private List<Vector2> FreePos;

    [SerializeField] private Vector2Int RowColumnSize;

    private Grid grid;

    protected override void Awake()
    {
        gameObject.transform.position = new Vector3(-0.5f + 2f, 0, -0.5f);
        grid = gameObject.GetComponent<Grid>();
    }

    protected override void Start()
    {
        AttackType = AttackTypeEnum.SpawnALot;
        PrepareDataToAttack();
    }

    protected override void Update()
    {
        
    }

    protected override void PrepareDataToAttack()
    {
        FreePos = new List<Vector2>();
        spawned = new List<GameObject>();
        var deltaZ = RowColumnSize.y / 2;
        for(int i = 0; i < RowColumnSize.x; i++)
        {
            for(int j = 0; j < RowColumnSize.y; j++)
            {
                FreePos.Add(new Vector2(i, j - deltaZ));
            }
        }
        ActivateAttack();
    }

    protected override void ActivateAttack()
    {
        StartCoroutine(SpawnEnemyRandom());
    }

    public override void KillAll()
    {
        foreach (var enemy in spawned)
        {
            IDamageable EnemyToDamage;
            if (TryGetComponent(out EnemyToDamage))
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

    private IEnumerator SpawnEnemyRandom()
    {
        for (int i = 0; i < RowColumnSize.x*RowColumnSize.y; i++)
        {
            var enemySpawned = Instantiate(EnemyToSpawn[Random.Range(0, EnemyToSpawn.Count)]);
            var randInt = Random.Range(0, FreePos.Count);
            Vector2 StayPos = new Vector2(FreePos[randInt].x, FreePos[randInt].y);
            FreePos.RemoveAt(randInt);
            spawned.Add(enemySpawned);
            enemySpawned.name = enemySpawned.name + StayPos.x + "," + StayPos.y;
            enemySpawned.transform.position = EnemySpawnPositions.Instance.GetRandomSP();
            enemySpawned.GetComponent<EnemyScript>().SpawnPos = enemySpawned.transform.position;
            enemySpawned.GetComponent<EnemyScript>().Zero = grid.CellToWorld(new Vector3Int((int)StayPos.x, (int)StayPos.y));//new Vector3(StayPos.x, 0, StayPos.y);//
            enemySpawned.GetComponent<EnemyScript>().OnEnemyDieEvent += Boss1AttackSpawnEnemy_OnEnemyDieEvent;
            yield return null;
        }
        while(spawned.Count > 0)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    private void Boss1AttackSpawnEnemy_OnEnemyDieEvent(object sender, System.EventArgs e)
    {
        spawned.Remove(((EnemyScript)sender).gameObject);
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
