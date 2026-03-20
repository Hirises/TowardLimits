using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitX3 : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX3;

    protected override UniTask SkillLoop(CancellationToken ct){
        ShootBullet(slot.position.y, 3);
        return UniTask.CompletedTask;
    }
}
