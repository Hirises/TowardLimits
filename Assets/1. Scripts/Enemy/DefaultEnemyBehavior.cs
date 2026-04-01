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

    private void Update(){
        if(isMoving){
            transform.position -= Vector3.forward * data.GetSpeed() * Time.deltaTime;
        }

        //정지 후 공격 검사
        if(data.rangeAttack && isMoving){
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
            await UniTask.Delay(TimeSpan.FromSeconds(1f / data.attackSpeed), cancellationToken: ct);
            Shoot();
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit")){
            UnitBehavior unit = other.gameObject.GetComponentInParent<UnitBehavior>();
            unit.TakeDamage(data.GetDamage());
            OnDeath();
        }
    }
}
