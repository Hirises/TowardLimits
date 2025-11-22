using System.Collections;
using UnityEngine;

/// <summary>
/// 일차 함수
/// </summary>
public class UnitX : UnitBehavior
{
    public override UnitType unitType => UnitType.UnitX;

    [SerializeField] private float attackSpeed;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    private Coroutine mainLoop;
    
    protected override void OnCombatStart_Internal(){
        mainLoop = StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop(){
        while(true){
            yield return new WaitForSeconds(1f / attackSpeed);
            OnShoot();
        }
    }

    public void OnShoot(){
        BulletBehavior bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.Shoot(attackRange, bulletSpeed, attackDamage);
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
