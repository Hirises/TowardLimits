using UnityEngine;

public class SnowBall : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.Snowball;

    public override void Shoot()
    {
        //pass
    }
}
