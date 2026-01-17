using UnityEngine;
using NaughtyAttributes;

public abstract class EnemyBehavior : LivingEntity
{
    public abstract EnemyType enemyType { get; }
    protected EnemyData data;
    [ReadOnly] public int health;

    private EntityVFX vfx;

    public void OnSummon(){
        data = enemyType.GetEnemyData();
        health = data.GetHealth();
        OnSummon_Internal();

        vfx = new EntityVFX();
        vfx.Initalize(this);
    }

    protected abstract void OnSummon_Internal();

    public void OnDeath(){
        OnDeath_Internal();
        data = null;
        CombatManager.instance.RemoveEnemy(this);
        vfx.Dispose();
        Destroy(gameObject);
    }

    protected abstract void OnDeath_Internal();

    protected override void TakeDamage_Internal(int damage){
        health -= damage;
        if(health <= 0){
            OnDeath();
        }
    }
}
