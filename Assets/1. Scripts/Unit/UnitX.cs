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
        for(int i = 0; i < 5; i++){
            OnShoot();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
        }
    }
}
