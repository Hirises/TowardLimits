using System.Collections;
using UnityEngine;
using NaughtyAttributes;

public abstract class UnitBehavior : MonoBehaviour
{
    public abstract UnitType unitType { get; }
    [ReadOnly] public int health;
    public int maxHealth;

    public void OnSummon(){
        health = maxHealth;
        OnSummon_Internal();
    }

    protected abstract void OnSummon_Internal();

    public abstract void OnDeath();

    public void TakeDamage(int damage){
        health -= damage;
        if(health <= 0){
            OnDeath();
        }
    }
}
