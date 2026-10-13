using UnityEngine;

public class CaptainPolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.CaptainPolarBear;

    [SerializeField] private SnowballStrike snowballStrikePrefab;

    private SnowballStrike activeStrike;

    public override void Shoot(){
        if(activeStrike != null) return;

        Slot[] slots = CombatManager.instance.slots;
        Slot targetSlot = slots[Random.Range(0, slots.Length)];
        activeStrike = Instantiate(snowballStrikePrefab, CombatManager.instance.enemyBulletRoot);
        activeStrike.StartStrike(targetSlot, data.GetRangeDamage());
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
