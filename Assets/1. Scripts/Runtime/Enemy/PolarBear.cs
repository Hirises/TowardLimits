using UnityEngine;

public class PolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.PolarBear;

    [SerializeField] private SnowballStrike snowballStrikePrefab;
    [SerializeField, Min(1)] private int attacksPerStrike = 3;

    private int attackCount;
    private SnowballStrike activeStrike;

    protected override void OnSummon_Internal(){
        attackCount = 0;
        base.OnSummon_Internal();
    }

    public override void Shoot(){
        int ran_line = Random.Range(0, 5);
        CombatManager.instance.SummonEnemy(EnemyType.Snowball, ran_line, transform.position.z);
        attackCount++;
        if(attackCount % attacksPerStrike == 0 && activeStrike == null){
            Slot[] slots = CombatManager.instance.slots;
            Slot targetSlot = slots[Random.Range(0, slots.Length)];
            activeStrike = Instantiate(snowballStrikePrefab, CombatManager.instance.enemyBulletRoot);
            activeStrike.StartStrike(targetSlot, data.GetRangeDamage());
        }
    }

    protected override void OnDeath_Internal(){
        CancelStrike();
        base.OnDeath_Internal();
    }

    private void OnDestroy(){
        CancelStrike();
    }

    private void CancelStrike(){
        if(activeStrike != null){
            activeStrike.Cancel();
            activeStrike = null;
        }
    }
}
