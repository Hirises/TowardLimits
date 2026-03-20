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

    public override void OnShoot(){
        ShootBullet(slot.position.y);
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.data.damageRatio, 0.7f);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.data.damageRatio, 0.7f);
    }

    protected override async UniTask SkillLoop(CancellationToken ct){
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.data.damageRatio, 0.7f);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.data.damageRatio, 0.7f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
        ShootBullet(slot.position.y);
    }
}
