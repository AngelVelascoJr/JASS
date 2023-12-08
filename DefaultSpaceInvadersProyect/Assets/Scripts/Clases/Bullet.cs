using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    protected float Damage = 10f;
    public virtual float Velocity => 5f;
    public string Caller;
    public Vector3 Direction { get; set; }
    public GameObject PlayerGameObject;
    public Vector3 Dir { get; set; }
    public Vector3 PrevPos { get; set; }
    public InteractiveTypeOf TheTypeOf { get; set; }
    private AudioSource Audio;

    public enum InteractiveTypeOf
    {
        Player,
        Enemy
    }

    protected void Start()
    {
        if (PlayerController.Instance != null)
            PlayerGameObject = PlayerController.Instance.gameObject;
        else
            PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        Audio = GetComponent<AudioSource>();
        StartCoroutine(RemoveAudiosource());
    }

    public virtual void FixedUpdate()
    {
        GoToPos();
        if (Vector3.Distance(gameObject.transform.position, /*PlayerController.Instance.*/PlayerGameObject.transform.position) > 20)
            DestroyBullet();
    }
    public virtual void SetBullet(Vector3 Destination, float vel, InteractiveTypeOf TheTypeOf, string caller, float Damage = 10f)
    {
        this.Direction = Destination;
        this.TheTypeOf = TheTypeOf;
        this.Caller = caller;
        this.Damage = Damage;
        //InitialRotation();
        PrevPos = gameObject.transform.position;
    }

    protected void InitialRotation()
    {
        Quaternion LookRotation = Quaternion.FromToRotation(gameObject.transform.up, Direction);
        transform.rotation = LookRotation;
    }

    protected virtual void GoToPos()
    {
        transform.Translate(Velocity * Direction * Time.deltaTime, Space.Self);
    }

    public virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }

    public virtual float GetDamage()
    {
        return Damage;
    }

    public virtual InteractiveTypeOf GetInteractiveTypeOf()
    {
        return TheTypeOf;
    }

    IEnumerator RemoveAudiosource()
    {
        while(Audio.isPlaying)
            yield return null;
        Destroy(Audio);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("COLISION");
        if(other.gameObject.GetComponent<IDamageable>() != null && !other.gameObject.CompareTag(Caller))
        {
            other.gameObject.GetComponent<IDamageable>().GetHealthClass().WasHitted(Damage);
            Destroy(gameObject);
        }
    }

}
