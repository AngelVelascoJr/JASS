using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HealthClass
{
    public event EventHandler<OnHealthChangeArgs> OnHealthChange;
    public event EventHandler OnDieEvent;
    public int HEALTH_MAX { get; private set; }
    [SerializeField] private float CurrentHealth;
    private float HealthRegenAmount;
    private float TimeToStartRegen;
    private float DeltaTime;
    private bool CanRegenerate;
    private Gradient HealthGradient;
    //private int maxHealth;

    public HealthClass(Gradient HealthGradient, int HEALTH_MAX = 100, float CurrentHealth = 100f, bool CanRegenerate = true)
    {
        this.CurrentHealth = CurrentHealth;
        this.HEALTH_MAX = HEALTH_MAX;
        HealthRegenAmount = 30f;
        TimeToStartRegen = 4f;
        this.CanRegenerate = CanRegenerate;
        this.HealthGradient = HealthGradient;
    }

    public HealthClass(int HEALTH_MAX, int CurrentHealth)
    {
        this.HEALTH_MAX = HEALTH_MAX;
        this.CurrentHealth = CurrentHealth;
    }

    public HealthClass(int maxHealth)
    {
        this.HEALTH_MAX = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void Update(HealthClass PHealth)
    {
        if(CanRegenerate)
        {
            if (DeltaTime >= TimeToStartRegen && CurrentHealth < HEALTH_MAX)
            {
                CurrentHealth += HealthRegenAmount * Time.deltaTime;
                CurrentHealth = Mathf.Clamp(CurrentHealth, -1, HEALTH_MAX);
                OnHealthChange?.Invoke(this, new OnHealthChangeArgs { Health = PHealth });
            }
            else
            {
                DeltaTime += Time.deltaTime;
            }
        }
    }

    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public float GetCurrentHealthNormalized()
    {
        return CurrentHealth / HEALTH_MAX;
    }

    public Color GetHealthGradientValue()
    {
        return HealthGradient.Evaluate(GetCurrentHealthNormalized());
    }

    public void RestoreAllHealth()
    {
        if (!IsDead())
        {
            DeltaTime = TimeToStartRegen;
            CurrentHealth = HEALTH_MAX;
            OnHealthChange?.Invoke(this, new OnHealthChangeArgs { Health = this });
        }
    }

    public void WasHitted(float Damage)
    {
        DeltaTime = 0;
        if (GetCurrentHealth() - Damage <= 0)        //
            CurrentHealth = 0;
        else
        {            
            CurrentHealth -= Damage;
        }
        OnHealthChange?.Invoke(this, new OnHealthChangeArgs { Health = this });
        if (IsDead())
        {
            OnDieEvent?.Invoke(this, EventArgs.Empty);
            CanRegenerate = false;
            CurrentHealth = 0;
        }
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAH ME PEGOOOOOOOOOOOOOOOOOOOOO");
    }

    public bool IsDead()
    {
        return GetCurrentHealth() <= 0;
    }

    public class OnHealthChangeArgs : EventArgs
    {
        public HealthClass Health;
    }

}
