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
        //계수만큼 추가 발사 & 회복
        vfx.StartSkillLoopVFX();
        for(int i = 0; i < 3; i++)
        {
            CombatManager.instance.GetSlotAt(slot.position.x + i, slot.position.y)?.unit?.Heal(status.model.healAmount);
        }
        for(int i = 0; i < status.level; i++){
            vfx.ShowAttackSkillCount(i + 1);
            OnShoot();
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
        }
        vfx.StopSkillLoopVFX();
    }
}
