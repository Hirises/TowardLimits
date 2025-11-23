using UnityEngine;

public class BulletX3Behavior : BulletBehavior
{
    private int count;
    
    public override void Shoot(float distance, float speed, int damage){
        base.Shoot(distance, speed, damage);
        count = GameManager.instance.GetUnitData(UnitType.UnitX3).attackCount;
    }

    public override void OnHitEnemy(EnemyBehavior enemy){
        enemy.TakeDamage(damage);
        count--;
        if(count <= 0){
            Destroy(gameObject);
        }
    }
}
