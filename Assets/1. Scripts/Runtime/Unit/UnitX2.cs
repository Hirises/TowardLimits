using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 이차함수
/// </summary>
public class UnitX2 : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX2;

    public override void OnShoot()
    {
        ShootTask().Forget();
    }

    private async UniTask ShootTask()
    {
        ShootBullet(slot.position.y);
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: this.GetCancellationTokenOnDestroy());
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.model.damageRatio, 0.7f);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.model.damageRatio, 0.7f);
    }

    protected override async UniTask SkillLoop(CancellationToken ct){
        //계수만큼 추가 발사
        vfx.StartSkillLoopVFX();
        for(int i = 0; i < status.level; i++){
            vfx.ShowAttackSkillCount(i + 1);
            OnShoot();
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
        }
        vfx.StopSkillLoopVFX();
    }
}
