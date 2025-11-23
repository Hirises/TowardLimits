using UnityEngine;

public class PolarBear : EnemyBehavior
{
    public override EnemyType enemyType => EnemyType.PolarBear;

    protected override void OnSummon_Internal()
    {

    }

    protected override void OnDeath_Internal()
    {

    }

    private void Update(){
        transform.position -= Vector3.forward * data.speed * Time.deltaTime;
        if(transform.position.z <= -10){
            CombatManager.instance.Persuade(data.persuade);
            OnDeath();
        }
    }
}
