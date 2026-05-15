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
        ApplyBuff();
    }

    private void ApplyBuff()
    {
        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++){
                ApplyBuffAt(CombatManager.instance.GetSlotAt(slot.position.x + x, slot.position.y + y));
            }
        }
    }

    private void RemoveBuff(){
        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++){
                RemoveBuffAt(CombatManager.instance.GetSlotAt(slot.position.x + x, slot.position.y + y));
            }
        }
    }

    private void ApplyBuffAt(Slot slot){
        if(slot == null){
            return;
        }
        slot.DMG_buff += status.model.DMG_buff;
        slot.ATKSPD_buff += status.model.ATKSPD_buff;
    }

    private void RemoveBuffAt(Slot slot){
        if(slot == null){
            return;
        }
        slot.DMG_buff -= status.model.DMG_buff;
        slot.ATKSPD_buff -= status.model.ATKSPD_buff;
    }

    protected override void PerformSkill_Internal(){
        skillLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        SkillLoop(skillLoop.Token).Forget();
    }

    private async UniTask SkillLoop(CancellationToken ct){
        vfx.StartSkillLoopVFX();
        ApplyBuff();
        await UniTask.Delay(TimeSpan.FromSeconds(status.model.buffDuration), cancellationToken: ct);
        RemoveBuff();
        vfx.StopSkillLoopVFX();
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
        RemoveBuff();
    }
}
