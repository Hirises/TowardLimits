using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class DefaultUnitBehavior : UnitBehavior
{
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    private CancellationTokenSource mainLoop;
    private CancellationTokenSource skillLoop;
    
    protected override void OnCombatStart_Internal(){
        mainLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        MainLoop(mainLoop.Token).Forget();
    }

    private async UniTask MainLoop(CancellationToken ct){
        while(true){
            await UniTask.Delay(TimeSpan.FromSeconds(1f / (status.model.attackSpeed + slot.ATKSPD_buff)), cancellationToken: ct);
            OnShoot();
        }
    }

    public virtual void OnShoot(){
        ShootBullet(slot.position.y);
    }

    public void ShootBullet(int column){
        ShootBullet(column, 1f);
    }

    public void ShootBullet(int column, float damageRatio, float scale = 1f){
        int damage = Mathf.RoundToInt((status.model.attackRange + slot.DMG_buff) * damageRatio);
        Vector3 position = new Vector3(RelavtiveLineHandler.instance.ColumnX(column), bulletSpawnPoint.position.y, bulletSpawnPoint.position.z);
        BulletBehavior bullet = Instantiate(bulletPrefab, position, Quaternion.identity, CombatManager.instance.bulletRoot);
        bullet.transform.localScale *= scale;
        bullet.Shoot(damage, status.model.bulletSpeed + slot.BULLETSPD_buff, status.CurrentAttack + slot.DMG_buff);
    }

    public override void OnCombatEnd(){
        if(mainLoop != null){
            mainLoop.Cancel();
            mainLoop.Dispose();
            mainLoop = null;
        }
        if(skillLoop != null){
            skillLoop.Cancel();
            skillLoop.Dispose();
            skillLoop = null;
        }
    }

    protected override void OnPlacement_Internal(){
        Debug.Log($"{unitType} OnPlacement");
    }

    protected override void OnDisplacement_Internal(){
        Debug.Log($"{unitType} OnDisplacement");
    }

    protected override void PerformSkill_Internal(){
        skillLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        SkillLoop(skillLoop.Token).Forget();
    }

    protected abstract UniTask SkillLoop(CancellationToken ct);
}
