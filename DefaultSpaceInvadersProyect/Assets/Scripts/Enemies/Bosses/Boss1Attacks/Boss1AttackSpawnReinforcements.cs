using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class Boss1AttackSpawnReinforcements : BossAttackBase
{

    public List<GameObject> EnemyToSpawn;
    private List<GameObject> spawned;
    private List<Vector2> FreePos;

    [SerializeField] private Vector2Int RowColumnSize;

    private GameObject Boss;
    private Grid grid;

    protected override void Awake()
    {
        grid = GetComponent<Grid>();
        Boss = GameObject.FindGameObjectWithTag("Boss");
        gameObject.transform.position = Boss.transform.position;// + new Vector3(-0.5f, 0, -0.5f);
    }

    protected override void Start()
    {
        AttackType = AttackTypeEnum.ReinforcementsNearBoss;
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
        for (int i = 0; i < RowColumnSize.x; i++)
        {
            for (int j = 0; j < RowColumnSize.y; j++)
            {
                if(i == 0 && j - deltaZ == 0)
                {
                    continue;
                }
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
        for (int i = 0; i < (RowColumnSize.x * RowColumnSize.y) - 1; i++)
        {
            var enemySpawned = Instantiate(EnemyToSpawn[Random.Range(0, EnemyToSpawn.Count)]);
            var randInt = Random.Range(0, FreePos.Count);
            Vector2 StayPos = new Vector2(FreePos[randInt].x, FreePos[randInt].y);
            FreePos.RemoveAt(randInt);
            spawned.Add(enemySpawned);
            enemySpawned.name = enemySpawned.name + StayPos.x + "," + StayPos.y;
            enemySpawned.transform.position = EnemySpawnPositions.Instance.GetRandomSP();
            var enemySpawnedComp = enemySpawned.GetComponent<EnemyScript>();
            enemySpawnedComp.SpawnPos = enemySpawned.transform.position;
            enemySpawnedComp.Zero = grid.CellToWorld(new Vector3Int((int)StayPos.x, (int)StayPos.y));
            enemySpawnedComp.OnEnemyDieEvent += Boss1AttackSpawnReinforcements_OnEnemyDieEvent;
            enemySpawnedComp.ShootControllerProperty.FollowTarget = true;
            enemySpawnedComp.ShootControllerProperty.target = PlayerController.Instance.gameObject;
            yield return null;
        }
        while (spawned.Count > 0)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    private void Boss1AttackSpawnReinforcements_OnEnemyDieEvent(object sender, System.EventArgs e)
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

