using System;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    public Action<int> onBeforeTakeDamage;
    
    public void TakeDamage(int damage){
        onBeforeTakeDamage?.Invoke(damage);
        TakeDamage_Internal(damage);
    }

    protected abstract void TakeDamage_Internal(int damage);
}
