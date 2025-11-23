using UnityEngine;

/// <summary>
/// 이차함수
/// </summary>
public class UnitX2 : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX2;

    public override void OnShoot(){
        ShootBullet(slot.position.y);
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.data.damageRatio);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.data.damageRatio);
    }
}
