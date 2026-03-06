using System.Collections;
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

    protected override IEnumerator SkillLoop(){
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.data.damageRatio, 0.7f);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.data.damageRatio, 0.7f);
        yield return new WaitForSeconds(0.1f);
        ShootBullet(slot.position.y);
    }
}
