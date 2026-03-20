using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitC : UnitBehavior
{
    public override UnitType unitType => UnitType.UnitC;
    private CancellationTokenSource skillLoop;

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
        skillLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        SkillLoop(skillLoop.Token).Forget();
    }

    private async UniTask SkillLoop(CancellationToken ct){
        OnPlacement_Internal();
        await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: ct);
        OnDisplacement_Internal();
    }

    protected override void OnCombatStart_Internal()
    {

    }



    public override void OnCombatEnd()
    {
        if(skillLoop != null){
            skillLoop.Cancel();
            skillLoop.Dispose();
            skillLoop = null;
        }
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
