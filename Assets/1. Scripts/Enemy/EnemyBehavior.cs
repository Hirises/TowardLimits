using UnityEngine;
using NaughtyAttributes;

public abstract class EnemyBehavior : LivingEntity
{
    public abstract EnemyType enemyType { get; }
    protected EnemyData data;
    [ReadOnly] public int health;

    public void OnSummon(){
        data = enemyType.GetEnemyData();
        health = data.GetHealth();
        OnSummon_Internal();
    }

    protected abstract void OnSummon_Internal();

    public void OnDeath(){
        OnDeath_Internal();
        data = null;
        CombatManager.instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    protected abstract void OnDeath_Internal();

    public override void TakeDamage(int damage){
        health -= damage;
        if(health <= 0){
            OnDeath();
        }
    }
}
