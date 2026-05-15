using DG.Tweening;
using UnityEngine;

/// <summary>
/// 유닛 VFX 처리용 클래스
/// 당장은 아무런 추가 기능이 없다
/// </summary>
public class UnitVFX : EntityVFX {
    private UnitBehavior unit;

    private GameObject skillLoopVFXInstance;

    public UnitVFX(UnitBehavior unit) : base(unit){
        this.unit = unit;
        unit.onSkillPerform += ShowSkillActiveVFX;
    }

    public override void Dispose(){
        unit = null;
        if(unit != null){
            unit.onSkillPerform -= ShowSkillActiveVFX;
        }
        base.Dispose();
    }

    public void ShowSkillActiveVFX(){
        var inst = GameObject.Instantiate(ResourceHolder.Instance.skillActiveVFXPrefab, unit.Pivot.transform);
        GameObject.Destroy(inst, 1f);
    }

    public void StartSkillLoopVFX()
    {
        StopSkillLoopVFX(); // 기존 이펙트 제거
        skillLoopVFXInstance = GameObject.Instantiate(ResourceHolder.Instance.skillLoopVFXPrefab, unit.Pivot.transform);
    }

    public void StopSkillLoopVFX()
    {
        if (skillLoopVFXInstance != null)
        {
            GameObject.Destroy(skillLoopVFXInstance);
            skillLoopVFXInstance = null;
        }
    }

    public void ShowAttackSkillCount(int count)
    {        
        var inst = GameObject.Instantiate(ResourceHolder.Instance.attackSkillCountVFXPrefab, unit.transform.position, Quaternion.identity);
        inst.transform.position = unit.transform.position;
        inst.Show(count);
        //얘는 알아서 Destroy 함
    }
}