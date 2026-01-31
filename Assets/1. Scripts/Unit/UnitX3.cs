using System.Collections;
using UnityEngine;

public class UnitX3 : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX3;

    protected override IEnumerator SkillLoop(){
        ShootBullet(slot.position.y, 3);
        yield return null;
    }
}
