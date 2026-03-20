using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitABS : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitABS;

    public override void OnShoot(){
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1);
    }

    protected override async UniTask SkillLoop(CancellationToken ct){
        for(int i = 0; i < 5; i++){
            OnShoot();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
        }
    }
}
