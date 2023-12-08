using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShootController
{

    public event System.EventHandler OnShootEvent;
    private int Vel = 2;
    [field:SerializeField] public bool Shoot { get; private set; }
    [field: SerializeField] public bool CanShoot { get; private set; }
    [field:SerializeField] public bool AllowRandomShoot { get; private set; }
    [SerializeField] public bool FollowTarget = false;
    [SerializeField] public GameObject target;
    [SerializeField] private int ShootProb = 30;
    private bool RandomShoot = false;
    private float Counter = 0;
    private float COUNTER_MAX = 0;
    private BulletType BulletTypeVar { get; set; }
    [SerializeField] private Bullet.InteractiveTypeOf typeOfInteraction;
    [SerializeField] private float BulletDamage = 10f;

    public string caller;
    public GameObject[] Launchers = new GameObject[6];
    public GameObject[] Bullets = new GameObject[3];
    public enum BulletType
    {
        Normal,
        Triple
    }

    public ShootController(GameObject[] Launchers, GameObject[] Bullets, Bullet.InteractiveTypeOf typeOfInteraction, string caller)
    {
        this.Launchers = Launchers;
        this.Bullets = Bullets;
        this.typeOfInteraction = typeOfInteraction;
        BulletTypeVar = BulletType.Normal;
        Shoot = false;
        CanShoot = false;
        this.caller = caller;
    }

    public void Start(float CounterMax, bool randomStart = false, GameObject target = null)
    {
        COUNTER_MAX = CounterMax;
        //BulletDamage = Damage;
        this.target = target;
        if(randomStart)
        {
            Counter += Random.Range(0, CounterMax * (7 / 8));
        }
    }

    public void Start(float CounterMax, float Damage = 10f)
    {
        COUNTER_MAX = CounterMax;
        BulletDamage = Damage;
    }

    public void ManualShoot()
    {
        if(HasRecoveredShoot())
        {
            Shoot = true;
            Counter = 0;
        }
    }

    public void Update()
    {
        if (HasRecoveredShoot() && AllowRandomShoot)
        {
            RandomShoot = true;
            Counter = 0;
        }
        if (CanShoot)
        {
            if(AllowRandomShoot & RandomShoot)
            {
                if(Random.Range(0, 101) < ShootProb)
                    SpawnBullets();
                RandomShoot = false;
            }
            if(Shoot)
            {
                SpawnBullets();
                Shoot = false;
            }
        }
        Counter += Time.deltaTime;
    }

    private void SpawnBullets()
    {
        GameObject B;
        B = Object.Instantiate(Bullets[(int)BulletTypeVar], Launchers[0].transform.position, Quaternion.identity);
        B.GetComponent<Bullet>().SetBullet(Vector3.right, Vel, typeOfInteraction, caller, BulletDamage);
        B.layer = 9;
        if(caller != "Player")
        {
            B.transform.Rotate(0, 180f, 0);
            Counter += Random.Range(-COUNTER_MAX * (2 / 8), COUNTER_MAX * (2 / 8));
        }
        if (FollowTarget || (Random.Range(0f, 1f) <= 0.5f && AllowRandomShoot))
        {
            B.transform.Rotate(Quaternion.FromToRotation(B.transform.right, target.transform.position - Launchers[0].transform.position).eulerAngles);
            Counter += Random.Range(-COUNTER_MAX * (2 / 8), COUNTER_MAX * (2 / 8));
        }
        OnShootEvent?.Invoke(this, System.EventArgs.Empty);
    }

    public void ChangeBulletType(BulletType type)
    {
        BulletTypeVar = type;
    }

    public void SetManualShootValue(bool value)
    {
        Shoot = value;
    }

    public void SetCanShootValue(bool value)
    {
        CanShoot = value;
    }

    public bool HasRecoveredShoot()
    {
        return Counter > COUNTER_MAX;
    }
}
