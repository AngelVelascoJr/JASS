using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyScript : MonoBehaviour, IDamageable
{
    [field:Header("Health System")]
    [field: SerializeField] public HealthClass Health { get; private set; }
    //public Gradient HealthGradient;
    [SerializeField] private int MaxHealth = 100;
    [SerializeField] private GameObject DieParticleEfect;
    [SerializeField] private int Score;
    [field:Header("Shoot System")]
    [field: SerializeField] public ShootController ShootControllerProperty { get; private set; }
    [SerializeField] private BoxCollider Col;
    private AudioSource Audio;
    [SerializeField] private AudioClip ShootClip;
    [Header("Movement System")]
    public AnimationCurve MovementCurveX;
    [SerializeField] private float MovementCurveVelocityX;
    [SerializeField] private float MovementCurveAmplificationX;
    public AnimationCurve MovementCurveZ;
    [SerializeField] private float MovementCurveVelocityZ;
    [SerializeField] private float MovementCurveAmplificationZ;
    [SerializeField] private IAPatern patern;
    [Header("Fibonaccy System")]
    [SerializeField] private AnimationCurve FibonaccyX;
    [SerializeField] private AnimationCurve FibonaccyY;
    [SerializeField] public List<Vector3> FibonaccyMovementCurve;
    [SerializeField] private AnimationCurve FibonaccyMovementCurveX;
    [SerializeField] private AnimationCurve FibonaccyMovementCurveY;

    public event EventHandler OnEnemyDieEvent;

    /// <summary>
    /// The position where they beguin to attack
    /// </summary>
    public Vector3 Zero = new Vector3();
    /// <summary>
    /// The position where they spawn
    /// </summary>
    public Vector3 SpawnPos = new Vector3();
    //Vector3 RandomStartOffset = new Vector3();
    [SerializeField] public Vector3 Desplazamiento = new Vector3();

    private NavMeshAgent AIAgent;

    private enum IAPatern
    {
        Forward,
        SideToSide,
        Stay
    }

    private void OnDieFunc(object sender, System.EventArgs e)
    {
        Explote();
    }

    private void Awake()
    {
        Health = new HealthClass(MaxHealth);
        Health.OnDieEvent += OnDieFunc;
        ShootControllerProperty.caller = gameObject.tag;
        ShootControllerProperty.OnShootEvent += ShootControllerProperty_OnShootEvent;
        AIAgent = GetComponent<NavMeshAgent>();
        Audio = GetComponentInChildren<AudioSource>();
    }

    private void Boss_OnDieEvent(object sender, EventArgs e)
    {
        ShootControllerProperty.SetCanShootValue(false);
    }

    private void ShootControllerProperty_OnShootEvent(object sender, EventArgs e)
    {
        Audio.PlayOneShot(ShootClip);
    }

    private void Start()
    {
        BossScript.instance.Health.OnDieEvent += Boss_OnDieEvent;
        Zero += new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), 0f, UnityEngine.Random.Range(-0.7f, 0.7f));
        ShootControllerProperty.Start(0.7f, target : PlayerController.Instance.gameObject);
        CalculateFibonaccyPoints();
        StartCoroutine(MoveInFibonacciSpiral());
        MovementCurveVelocityX += UnityEngine.Random.Range(-MovementCurveVelocityX / 5, MovementCurveVelocityX / 5);
        if (patern == IAPatern.SideToSide || patern == IAPatern.Forward)
        {
            Col.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (patern)
        {
            case IAPatern.Forward:
                Desplazamiento.x = MovementCurveVelocityX * Time.deltaTime;
                Desplazamiento.z = MovementCurveZ.Evaluate(GameSceneController.Instance.UniversalTime * MovementCurveVelocityZ) * MovementCurveAmplificationZ * Time.deltaTime;
                transform.position = new Vector3(Desplazamiento.x, 0f, Desplazamiento.z) + transform.position;
                break;
            case IAPatern.SideToSide:
                Desplazamiento.x = MovementCurveX.Evaluate(GameSceneController.Instance.UniversalTime * MovementCurveVelocityX) * MovementCurveAmplificationX * Time.deltaTime;
                Desplazamiento.z = MovementCurveZ.Evaluate(GameSceneController.Instance.UniversalTime * MovementCurveVelocityZ) * MovementCurveAmplificationZ * Time.deltaTime;
                transform.position = new Vector3(Desplazamiento.x, 0f, Desplazamiento.z) + transform.position;
                break;
            case IAPatern.Stay:
                Desplazamiento.x = MovementCurveX.Evaluate(GameSceneController.Instance.UniversalTime * MovementCurveVelocityX) * MovementCurveAmplificationX * Time.deltaTime;
                Desplazamiento.z = MovementCurveZ.Evaluate(GameSceneController.Instance.UniversalTime * MovementCurveVelocityZ) * MovementCurveAmplificationZ * Time.deltaTime;
                transform.position = new Vector3(Desplazamiento.x, 0f, Desplazamiento.z) + Zero;
                break;
            default:
                break;
        }
        Health.Update(Health);
        ShootControllerProperty.Update();
    }

    private void Explote()
    {
        ShootControllerProperty.SetCanShootValue(false);
        var Effect = Instantiate(DieParticleEfect, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        var main = Effect.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(GetComponentInChildren<SpriteRenderer>().color);
        ScoreManager.Instance.AddScore(Score);
        OnEnemyDieEvent?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    void CalculateFibonaccyPoints()
    {
        FibonaccyMovementCurve = new List<Vector3>
        {
            SpawnPos
        };
        var TotalDistance = Vector3.Distance(SpawnPos, Zero);
        var FibonaccyDistance = Vector3.Distance(new Vector3(FibonaccyX[0].value, FibonaccyY[0].value), new Vector3(FibonaccyX[5].value, FibonaccyY[5].value));
        var Relation = TotalDistance / FibonaccyDistance;
        FibonaccyMovementCurveX.AddKey(0, SpawnPos.x);
        FibonaccyMovementCurveY.AddKey(0, SpawnPos.z);
        for (int i = 1; i <= 4 ; i++)
        {
            var FibonaccyVectorStart = new Vector3(FibonaccyX[0].value, 0, FibonaccyY[0].value);
            var FibonaccyVectorEnd = new Vector3(FibonaccyX[5].value, 0, FibonaccyY[5].value);
            var RotAngle = Vector3.Angle(Zero - SpawnPos, FibonaccyVectorEnd - FibonaccyVectorStart);
            FibonaccyMovementCurve.Add(Relation * (Quaternion.AngleAxis(RotAngle, Vector3.up) * new Vector3(FibonaccyX[i].value, 0, FibonaccyY[i].value)/*(Quaternion.Euler(0, RotAngle, 0) * new Vector3(FibonaccyX[i].value, 0, FibonaccyY[i].value))*/));
            FibonaccyMovementCurveX.AddKey(i, (Quaternion.Euler(
                0,
                RotAngle,
                0
                ) * FibonaccyMovementCurve[i]).x);
            FibonaccyMovementCurveY.AddKey(i, (Quaternion.Euler(
                0,
                Vector3.Angle(
                    Zero - SpawnPos,
                    new Vector3(
                        FibonaccyX[5].value,
                        FibonaccyY[5].value)
                    - new Vector3(
                        FibonaccyX[0].value,
                        FibonaccyY[0].value)
                    ),
                0
                ) * FibonaccyMovementCurve[i]).z);
        }
        FibonaccyMovementCurve.Add(Zero);
        FibonaccyMovementCurveX.AddKey(FibonaccyMovementCurve.Count - 1, FibonaccyMovementCurve[FibonaccyMovementCurve.Count - 1].x);
        FibonaccyMovementCurveY.AddKey(FibonaccyMovementCurve.Count - 1, FibonaccyMovementCurve[FibonaccyMovementCurve.Count - 1].z);
    }

    IEnumerator MoveInFibonacciSpiral()
    {
        yield return null;
        var count = 0f;//(float)FibonaccyMovementCurve.Count;
        var speed = 1.7f + UnityEngine.Random.Range(0, 3);
        while (Vector3.Distance(transform.position, FibonaccyMovementCurve[FibonaccyMovementCurve.Count - 1]) > 0.1f)
        {
            transform.position = new Vector3(FibonaccyMovementCurveX.Evaluate(count), 0, FibonaccyMovementCurveY.Evaluate(count));
            count += Time.deltaTime * speed;
            yield return null;
        }
        if(patern == IAPatern.SideToSide || patern == IAPatern.Forward)
        {
            Col.enabled = true;
        }
    }

    public HealthClass GetHealthClass()
    {
        return Health;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyScript))]
public class MyExampleScriptEditor : Editor
{
    void OnSceneGUI()
    {
        EnemyScript myEnemyScriptRef = target as EnemyScript;
        var tr = myEnemyScriptRef.transform;

        var colorOrange = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = colorOrange;

        EditorGUI.BeginChangeCheck();

        Vector3 newVector = new Vector3(myEnemyScriptRef.Desplazamiento.x, 0, myEnemyScriptRef.Desplazamiento.z);
        Handles.DrawWireDisc(newVector, tr.forward, 0.5f);
        Handles.Label(newVector, newVector.ToString());
        for(int i = 0; i < myEnemyScriptRef.FibonaccyMovementCurve.Count; i++)//foreach(var pos in myEnemyScriptRef.FibonaccyMovementCurve)
        {
            var pos = myEnemyScriptRef.FibonaccyMovementCurve[i];
            Handles.DrawWireDisc(pos, tr.up, 0.5f);
            Handles.Label(pos, i.ToString());
        }

        EditorGUI.EndChangeCheck();
    }
}

#endif
