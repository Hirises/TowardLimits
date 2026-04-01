using UnityEngine;

public class PolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.PolarBear;

    public override void Shoot(){
        int ran_line = Random.Range(0, 5);
        Vector3 spawnPos = new Vector3(RelavtiveLineHandler.instance.ColumnX(ran_line), bulletSpawnPoint.position.y, bulletSpawnPoint.position.z);
        EnemyBulletBehaviour bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity, CombatManager.instance.enemyBulletRoot);
        bullet.Shoot(data.GetSpeed(), data.GetDamage());
    }
}
