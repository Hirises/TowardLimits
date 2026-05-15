using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 일차 함수
/// </summary>
public class UnitX : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX;

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
