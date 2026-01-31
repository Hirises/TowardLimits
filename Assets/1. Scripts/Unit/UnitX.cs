using System.Collections;
using UnityEngine;

/// <summary>
/// 일차 함수
/// </summary>
public class UnitX : DefaultUnitBehavior
{
    public override UnitType unitType => UnitType.UnitX;

    protected override IEnumerator SkillLoop(){
        for(int i = 0; i < 5; i++){
            OnShoot();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
