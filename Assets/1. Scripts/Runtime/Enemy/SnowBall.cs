using UnityEngine;

public class SnowBall : DefaultEnemyBehavior
{
    [SerializeField] private EnemyType snowballType = EnemyType.Snowball;
    public override EnemyType enemyType => snowballType;

    protected override void OnSummon_Internal()
    {
        if(snowballType == EnemyType.Snowball2Line){
            Vector3 position = transform.position;
            position.x = (RelavtiveLineHandler.instance.ColumnX(line)
                + RelavtiveLineHandler.instance.ColumnX(line + 1)) * 0.5f;
            transform.position = position;
        }
        base.OnSummon_Internal();
    }

    public override void Shoot()
    {
        //pass
    }
}
