using System;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Action<int> onBeforeTakeDamage;
    
    public void TakeDamage(int damage){
        onBeforeTakeDamage?.Invoke(damage);
        TakeDamage_Internal(damage);
    }

    protected abstract void TakeDamage_Internal(int damage);
}
