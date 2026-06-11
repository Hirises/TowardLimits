using UnityEngine;
using Sirenix.OdinInspector;

public abstract class EnemyBehavior : LivingEntity
{
    public abstract EnemyType enemyType { get; }
    [SerializeField] protected EnemyBulletBehaviour bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    protected EnemyModel data;
    [ReadOnly] public int health;
    protected bool isMoving = true;
    protected int line;

    private EnemyVFX vfx;

    public void OnSummon(int line){
        data = enemyType.GetEnemyModel();
        health = data.GetHealth();
        isMoving = true;
        this.line = line;
        OnSummon_Internal();

        vfx = new EnemyVFX(this);
    }

    protected abstract void OnSummon_Internal();

    public virtual void Shoot(){
        EnemyBulletBehaviour bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity, CombatManager.instance.enemyBulletRoot);
        bullet.Shoot(data.GetSpeed(), data.GetDamage());
    }

    public void OnDeath(){
        OnDeath_Internal();
        data = null;
        CombatManager.instance.RemoveEnemy(this);
        vfx.Dispose();
        Destroy(gameObject);
    }

    protected abstract void OnDeath_Internal();

    protected override int TakeDamage_Internal(int damage, DamageType type){
        int finalDamage = damage;
        if(type == DamageType.Zero){
            finalDamage = Mathf.RoundToInt(damage * (100f / (100 + data.zeroDEF)));
        }
        else if(type == DamageType.Infinite){
            finalDamage = Mathf.RoundToInt(damage * (100f / (100 + data.infDEF)));
        }
        health -= finalDamage;
        if(health <= 0){
            OnDeath();
            return finalDamage;
        }
        return finalDamage;
    }

    protected override int Heal_Internal(int amount){
        health += amount;
        if(health > data.GetHealth()) health = data.GetHealth();
        return amount;
    }
}
