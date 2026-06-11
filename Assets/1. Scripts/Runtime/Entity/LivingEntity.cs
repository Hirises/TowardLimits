using System;
using UnityEngine;

/// <summary>
/// 살아있는 Entity
/// </summary>
public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected GameObject pivot;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public GameObject Pivot => pivot;
    public Action<int, DamageType> onBeforeTakeDamage;
    public Action<int, DamageType> onAfterTakeDamage;
    public Action<int> onBeforeHeal;
    public Action<int> onAfterHeal;
    
    public void TakeDamage(int damage, DamageType type){
        onBeforeTakeDamage?.Invoke(damage, type);
        int finalDamage = TakeDamage_Internal(damage, type);
        onAfterTakeDamage?.Invoke(finalDamage, type);
    }

    protected abstract int TakeDamage_Internal(int damage, DamageType type);

    public void Heal(int amount){
        onBeforeHeal?.Invoke(amount);
        int finalAmount = Heal_Internal(amount);
        onAfterHeal?.Invoke(finalAmount);
    }

    protected abstract int Heal_Internal(int amount);
}
