using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaitAndRunEnemyScript : MonoBehaviour
{

    public float Velocity;
    public Vector3 Direction;
    public bool GO = false;

    private float timer;
    private const int TIMER_MAX = 5;

    [SerializeField] private float DamageUponImpact;
    public event EventHandler OnEnemyDieEvent;
    [SerializeField] private GameObject DieParticleEfect;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.LookAt((gameObject.transform.position - Direction));
    }

    // Update is called once per frame
    void Update()
    {
        if(GO)
        {
            gameObject.transform.Translate(Velocity * Direction * Time.deltaTime);
            if(timer > TIMER_MAX)
            {
                OnEnemyDieEvent?.Invoke(this, EventArgs.Empty);
                Destroy(gameObject);
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>().GetHealthClass().WasHitted(DamageUponImpact);
            OnEnemyDieEvent?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(WaitAndRunEnemyScript))]
public class WaitAndRunEditor : Editor
{
    void OnSceneGUI()
    {
        WaitAndRunEnemyScript myEnemyScriptRef = target as WaitAndRunEnemyScript;
        var tr = myEnemyScriptRef.transform;

        var colorOrange = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = colorOrange;

        EditorGUI.BeginChangeCheck();

        Handles.DrawLine(tr.position, tr.position + myEnemyScriptRef.Direction);
        Handles.Label(tr.position + myEnemyScriptRef.Direction, myEnemyScriptRef.Velocity.ToString());

        EditorGUI.EndChangeCheck();
    }
}

#endif