using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DefaultEnemyBehavior : EnemyBehavior
{
    protected CancellationTokenSource attackLoop;

    protected override void OnSummon_Internal(){
        if(data.rangeAttack && data.attackWhileMoving){
            StartAttackLoop();
        }
    }

    protected override void OnDeath_Internal(){
        StopAttackLoop();
    }

    private void StartAttackLoop(){
        if(attackLoop != null) return;

        attackLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        if(data.attackWhileMoving && data.attackSpeed > 0){
            Shoot();
        }
        AttackLoop(attackLoop.Token).Forget();
    }

    private void StopAttackLoop(){
        if(attackLoop != null){
            attackLoop.Cancel();
            attackLoop.Dispose();
            attackLoop = null;
        }
    }

    private bool isLineEmpty(int line)
    {
        for(int i = 0; i < CombatManager.instance.girdSize.x; i++){
            if(CombatManager.instance.GetSlotAt(i, line)?.unit != null)
            {
                return false;
            }
        }
        return true;
    }

    private void Update(){
        bool lineEmpty = isLineEmpty(line);
        if(ShouldMove(lineEmpty)){
            transform.position -= Vector3.forward * data.GetSpeed() * Time.deltaTime;
        }
        isMoving = ShouldMove(lineEmpty);

        //사망 검사
        if(transform.position.z <= RelavtiveLineHandler.instance.BottomRowZ){
            CombatManager.instance.Persuade(data.persuade);
            OnDeath();
            return;
        }

        if(ShouldAttack(lineEmpty)){
            StartAttackLoop();
        }
        else{
            StopAttackLoop();
        }
    }

    private bool ShouldMove(bool lineEmpty){
        return !data.stopAtMiddle || lineEmpty
            || transform.position.z > RelavtiveLineHandler.instance.MiddleRowZ;
    }

    protected override float GetMovementSpeed(){
        return ShouldMove(isLineEmpty(line)) ? data.GetSpeed() : 0f;
    }

    private bool ShouldAttack(bool lineEmpty){
        return data.rangeAttack && (data.attackWhileMoving
            || (!isMoving && !lineEmpty));
    }

    protected async virtual UniTask AttackLoop(CancellationToken ct){
        while(true){
            if(data.attackSpeed <= 0)
            {
                await UniTask.Yield(cancellationToken: ct);
                continue;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f / data.attackSpeed), cancellationToken: ct);
            ct.ThrowIfCancellationRequested();
            if(ShouldAttack(isLineEmpty(line))){
                Shoot();
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit")){
            UnitBehavior unit = other.gameObject.GetComponentInParent<UnitBehavior>();
            unit.TakeDamage(data.GetDamage(), DamageType.None);
            OnDeath();
        }
    }
}
