using UnityEngine;

public class PolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.PolarBear;

    public override void Shoot(){
        EnemyType snowballType = data.enemy_polarbear_snowballType;
        int lineCount = CombatManager.instance.girdSize.y;
        int ran_line = Random.Range(0, snowballType == EnemyType.Snowball2Line ? lineCount - 1 : lineCount);
        CombatManager.instance.SummonEnemy(snowballType, ran_line, transform.position.z);
    }
}
