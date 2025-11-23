using UnityEngine;

public abstract class DefaultEnemyBehavior : EnemyBehavior
{
    protected override void OnSummon_Internal(){
    }

    protected override void OnDeath_Internal(){
    }

    private void Update(){
        transform.position -= Vector3.forward * data.GetSpeed() * Time.deltaTime;
        if(transform.position.z <= -10){
            CombatManager.instance.Persuade(data.persuade);
            OnDeath();
        }
    }
}
