using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public int startLife = 10;

    [SerializeField] private float _currentLife;
    public bool destroyOnKill = false;

    public Action<HealthBase> OnDamage;
    public Action<HealthBase> OnKill;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        destroyOnKill = false;
        _currentLife = startLife;

        ResetLife();
    }

    private void Update()
    {
        
    }

    protected void ResetLife()
    {
        _currentLife = startLife;
    }

    public void Damage(float amount)
    {
        _currentLife -= amount;

        if (_currentLife <= 0)
        {
            Kill();
        }

        OnDamage?.Invoke(this);
    }

    [NaughtyAttributes.Button]
    public void Damage()
    {
        Damage(5);
    }

    protected virtual void Kill()
    {
        if (destroyOnKill)
        {
            Destroy(gameObject, 2f);
        }

        OnKill?.Invoke(this);
    }
}
