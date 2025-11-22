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
            yield return new WaitForSeconds(1f / status.data.attackSpeed);
            OnShoot();
        }
    }

    public void OnShoot(){
        BulletBehavior bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.Shoot(status.data.attackRange, status.data.bulletSpeed, status.data.attack);
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
