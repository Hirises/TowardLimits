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

    /// <summary>
    /// 사망 처리가 시작되었는지 여부.
    /// Destroy는 프레임 끝에 적용되므로, 그 전까지 들어오는 피격/회복/사망 처리를 막는 데 사용한다.
    /// </summary>
    public bool IsDead { get; private set; }

    /// <summary>
    /// 사망 처리를 시작한다. 이미 사망 처리 중이면 false를 반환하므로 OnDeath 맨 앞에서 호출한다.
    /// </summary>
    protected bool TryMarkDead(){
        if(IsDead) return false;
        IsDead = true;
        return true;
    }

    public void TakeDamage(int damage, DamageType type){
        if(IsDead) return;
        onBeforeTakeDamage?.Invoke(damage, type);
        int finalDamage = TakeDamage_Internal(damage, type);
        onAfterTakeDamage?.Invoke(finalDamage, type);
    }

    protected abstract int TakeDamage_Internal(int damage, DamageType type);

    public void Heal(int amount){
        if(IsDead) return;
        onBeforeHeal?.Invoke(amount);
        int finalAmount = Heal_Internal(amount);
        onAfterHeal?.Invoke(finalAmount);
    }

    protected abstract int Heal_Internal(int amount);
}
