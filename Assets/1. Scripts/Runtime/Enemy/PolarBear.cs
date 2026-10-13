using UnityEngine;

public class PolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.PolarBear;

    public override void Shoot(){
        int ran_line = Random.Range(0, 5);
        CombatManager.instance.SummonEnemy(EnemyType.Snowball, ran_line, transform.position.z);
    }
}
