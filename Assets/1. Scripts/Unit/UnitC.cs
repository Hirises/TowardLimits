using System.Collections;
using UnityEngine;

public class UnitC : UnitBehavior
{
    public override UnitType unitType => UnitType.UnitC;
    private Coroutine skillLoop;

    protected override void OnPlacement_Internal()
    {
        ApplyBuff(slot);
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x, slot.position.y + 1));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x, slot.position.y - 1));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y + 1));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y + 1));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y - 1));
        ApplyBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y - 1));
    }

    private void ApplyBuff(Slot slot){
        if(slot == null){
            return;
        }
        slot.DMG_buff += status.data.DMG_buff;
        slot.ATKSPD_buff += status.data.ATKSPD_buff;
    }

    private void RemoveBuff(Slot slot){
        if(slot == null){
            return;
        }
        slot.DMG_buff -= status.data.DMG_buff;
        slot.ATKSPD_buff -= status.data.ATKSPD_buff;
    }

    protected override void PerformSkill_Internal(){
        skillLoop = StartCoroutine(SkillLoop());
    }

    private IEnumerator SkillLoop(){
        OnPlacement_Internal();
        yield return new WaitForSeconds(5f);
        OnDisplacement_Internal();
    }

    protected override void OnCombatStart_Internal()
    {

    }



    public override void OnCombatEnd()
    {
        if(skillLoop != null) StopCoroutine(skillLoop);
        skillLoop = null;
    }

    protected override void OnDisplacement_Internal()
    {
        RemoveBuff(slot);
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x, slot.position.y + 1));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x, slot.position.y - 1));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y + 1));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y + 1));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x + 1, slot.position.y - 1));
        RemoveBuff(CombatManager.instance.GetSlot(slot.position.x - 1, slot.position.y - 1));
    }
}
