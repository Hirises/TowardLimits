using System.Collections;
using UnityEngine;

public abstract class DefaultUnitBehavior : UnitBehavior
{
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    private Coroutine mainLoop;
    
    protected override void OnCombatStart_Internal(){
        mainLoop = StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop(){
        while(true){
            yield return new WaitForSeconds(1f / (status.data.attackSpeed + slot.ATKSPD_buff));
            OnShoot();
        }
    }

    public virtual void OnShoot(){
        ShootBullet(slot.position.y);
    }

    public void ShootBullet(int column){
        ShootBullet(column, 1f);
    }

    public void ShootBullet(int column, float damageRatio){
        int damage = Mathf.RoundToInt((status.data.attackRange + slot.DMG_buff) * damageRatio);
        Vector3 position = new Vector3(RelavtiveLineHandler.instance.ColumnX(column), bulletSpawnPoint.position.y, bulletSpawnPoint.position.z);
        BulletBehavior bullet = Instantiate(bulletPrefab, position, Quaternion.identity, CombatManager.instance.bulletRoot);
        bullet.Shoot(damage, status.data.bulletSpeed + slot.BULLETSPD_buff, status.data.attack + slot.DMG_buff);
    }

    public override void OnCombatEnd(){
        if(mainLoop != null) StopCoroutine(mainLoop);
        mainLoop = null;
    }

    protected override void OnPlacement_Internal(){
        Debug.Log($"{unitType} OnPlacement");
    }

    protected override void OnDisplacement_Internal(){
        Debug.Log($"{unitType} OnDisplacement");
    }
}
