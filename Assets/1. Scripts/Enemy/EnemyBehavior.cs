using UnityEngine;
using Sirenix.OdinInspector;

public abstract class EnemyBehavior : LivingEntity
{
    public abstract EnemyType enemyType { get; }
    protected EnemyModel data;
    [ReadOnly] public int health;

    private EnemyVFX vfx;

    public void OnSummon(){
        data = enemyType.GetEnemyModel();
        health = data.GetHealth();
        OnSummon_Internal();

        vfx = new EnemyVFX(this);
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
            return;
        }
        vfx.InvokeDamageEffect();
    }
}
