using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour, IDamageable
{

    public static PlayerController Instance { get; private set; }

    public PlayerInput PlayerInput { get; private set; }
    private AudioSource Audio;
    [SerializeField] private AudioClip ShootClip;

    [SerializeField] private float VelMax;
    [SerializeField] private float VelShoot;
    [SerializeField] private float Vel;
    [SerializeField] private Vector3 Movement;
    //[SerializeField] private bool _IsMouseLeftPressed = false;

    [SerializeField] private bool Shoot = false;

    [SerializeField] public HealthClass Health;
    public Gradient HealthGradient;
    [SerializeField] private int MaxHealth = 1000;
    [SerializeField] private GameObject DieParticleEfect;

    [field: SerializeField] public ShootController ShootControllerProperty { get; private set; }
    [SerializeField] private float ShootMax = 0.7f;
    [SerializeField] private float BulletDamage = 10f;

    [SerializeField] private CinemachineVirtualCamera VCam;

    private delegate void UpdateDelegate();
    UpdateDelegate FuncUpdateDelegate;

    private void Awake()
    {
        Health = new HealthClass(MaxHealth);
        ShootControllerProperty.caller = gameObject.tag;
        ShootControllerProperty.OnShootEvent += ShootControllerProperty_OnShootEvent;
        Instance = this;
        PlayerInput = GetComponent<PlayerInput>();
        Audio = GetComponentInChildren<AudioSource>();
        FuncUpdateDelegate += GameUpdateFunc;
        Health.OnHealthChange += OnHealthChangeFunc;
        Health.OnDieEvent += OnDieEvent;
    }

    private void Boss_OnDieEvent(object sender, System.EventArgs e)
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private void OnDieEvent(object sender, System.EventArgs e)
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        ShakeCamera(1.4f);
        StartCoroutine(DieCorroutine());
    }

    IEnumerator DieCorroutine()
    {
        yield return new WaitForSeconds(1.3f);
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        Instantiate(DieParticleEfect, gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    private void ShootControllerProperty_OnShootEvent(object sender, System.EventArgs e)
    {
        Audio.PlayOneShot(ShootClip);
    }

    private void OnHealthChangeFunc(object sender, HealthClass.OnHealthChangeArgs e)
    {
        ShakeCamera(0.5f);
        //if (Health.GetCurrentHealthNormalized() < 0.3f && !Audio.isPlaying)
        //{
        //    Audio.Play();
        //}
        //else
        //{
        //    Audio.Stop();
        //}
    }

    private void Start()
    {
        ShootControllerProperty.Start(ShootMax, BulletDamage);
        BossScript.instance.Health.OnDieEvent += Boss_OnDieEvent;
    }

    private void FixedUpdate()
    {
        FuncUpdateDelegate?.Invoke();
    }

    private void ShakeCamera(float time)
    {
        StartCoroutine(ShakeCorroutine(time));
    }

    IEnumerator ShakeCorroutine(float time)
    {
        CinemachineBasicMultiChannelPerlin Noice = VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Noice.m_AmplitudeGain = 0.4f;
        yield return new WaitForSeconds(time);
        Noice.m_AmplitudeGain = 0;
    }

    private void GameUpdateFunc()
    {
        if (PlayerInput.currentControlScheme == "Keyboard&Mouse" && Time.timeScale != 0 && !Health.IsDead())
        {
            transform.Translate(Movement * Time.deltaTime * Vel);            
        }
        if(Shoot)
        {
            ShootControllerProperty.ManualShoot();            
        }        
        Health.Update(Health);
        //Vel = VelMax;
        Vel = !ShootControllerProperty.HasRecoveredShoot() || Shoot ? VelShoot : VelMax;
        ShootControllerProperty.Update();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        //print(ctx.ReadValue<Vector2>());
        if (PlayerInput.camera != null && GameSceneController.Instance.OnPlayMode)
        {
            Movement = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        }
    }

    public void OnClic(InputAction.CallbackContext ctx)
    {
        if (!GameSceneController.Instance.OnPlayMode)
        {
            return;
        }

        if (ctx.performed || ctx.started)
        {
            //_IsMouseLeftPressed = true;
            Shoot = true;
            //Vel = VelShoot;
        }
        if (ctx.canceled)
        {
            Shoot = false;
            //_IsMouseLeftPressed = false;
            //Vel = VelMax;
        }
    }

    public HealthClass GetHealthClass()
    {
        return Health;
    }
}
