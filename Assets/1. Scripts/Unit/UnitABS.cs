using System.Collections;
using UnityEngine;

public class UnitABS : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitABS;

    public override void OnShoot(){
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1);
    }

    protected override IEnumerator SkillLoop(){
        for(int i = 0; i < 5; i++){
            OnShoot();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
