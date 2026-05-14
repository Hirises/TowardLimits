using System;
using UnityEngine;

/// <summary>
/// 살아있는 Entity
/// </summary>
public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Action<int> onBeforeTakeDamage;
    public Action<int> onBeforeHeal;
    
    public void TakeDamage(int damage){
        onBeforeTakeDamage?.Invoke(damage);
        TakeDamage_Internal(damage);
    }

    protected abstract void TakeDamage_Internal(int damage);

    public void Heal(int amount){
        onBeforeHeal?.Invoke(amount);
        Heal_Internal(amount);
    }

    protected abstract void Heal_Internal(int amount);
}
