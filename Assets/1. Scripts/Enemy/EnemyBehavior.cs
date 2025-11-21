using UnityEngine;
using NaughtyAttributes;

public abstract class EnemyBehavior : MonoBehaviour
{
    public abstract EnemyType enemyType { get; }
    [ReadOnly] public int health;
    public int maxHealth;

    public void OnSummon(){
        health = maxHealth;
        OnSummon_Internal();
    }

    protected abstract void OnSummon_Internal();

    public abstract void OnDeath();

    public void TakeDamage(int damage){
        Debug.Log($"{enemyType} TakeDamage: {damage}");
        health -= damage;
        if(health <= 0){
            Debug.Log($"{enemyType} OnDeath");
            OnDeath();
        }
    }
}
