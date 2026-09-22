using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DefaultEnemyBehavior : EnemyBehavior
{
    protected CancellationTokenSource attackLoop;

    protected override void OnSummon_Internal(){
    }

    protected override void OnDeath_Internal(){
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
        if(isMoving == false && isLineEmpty(line))
        {
            //라인이 비어있으면 다시 전진함
            isMoving = true;
        }

        if(isMoving){
            transform.position -= Vector3.forward * data.GetSpeed() * Time.deltaTime;
        }

        //정지 후 공격 검사
        if(data.rangeAttack && isMoving && !isLineEmpty(line)){
            if(transform.position.z <= RelavtiveLineHandler.instance.MiddleRowZ){
                isMoving = false;
                attackLoop = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
                AttackLoop(attackLoop.Token).Forget();
            }
        }

        //사망 검사
        if(transform.position.z <= RelavtiveLineHandler.instance.BottomRowZ){
            CombatManager.instance.Persuade(data.persuade);
            OnDeath();
        }
    }

    protected async virtual UniTask AttackLoop(CancellationToken ct){
        while(true){
            if(data.attackSpeed <= 0)
            {
                await UniTask.Yield(cancellationToken: ct);
                continue;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f / data.attackSpeed), cancellationToken: ct);
            Shoot();
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
