using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BossAttackBase : MonoBehaviour
{

    [SerializeField] protected AttackTypeEnum AttackType = AttackTypeEnum.ReinforcementsNearBoss;

    public enum AttackTypeEnum
    {
        SpawnALot,
        ReinforcementsNearBoss,
        FrenessyAttackFromTop,
        Attack2,
        Attack3,
        Attack4,
        Attack5
    }

    protected virtual void Awake()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    protected virtual void Start()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    protected virtual void Update()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    protected virtual void PrepareDataToAttack()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    protected virtual void ActivateAttack()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    public virtual void KillAll()
    {
        Debug.LogError("Falta Agregar override de la funcion");
    }

    public AttackTypeEnum GetAttackType()
    {
        return AttackType;
    }
}
