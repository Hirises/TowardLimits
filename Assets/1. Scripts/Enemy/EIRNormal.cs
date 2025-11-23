using UnityEngine;

public class EIRNormal : EnemyBehavior
{
    public override EnemyType enemyType => EnemyType.EIRNormal;

    protected override void OnSummon_Internal(){
    }

    protected override void OnDeath_Internal(){
    }

    private void Update(){
        transform.position -= Vector3.forward * data.speed * Time.deltaTime;
        if(transform.position.z <= -10){
            CombatManager.instance.Persuade(data.persuade);
            OnDeath();
        }
    }
}
