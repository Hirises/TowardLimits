using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaptainPolarBear : DefaultEnemyBehavior
{
    public override EnemyType enemyType => EnemyType.CaptainPolarBear;

    [SerializeField] private SnowballStrike snowballStrikePrefab;

    private readonly List<SnowballStrike> activeStrikes = new List<SnowballStrike>();

    public override void Shoot(){
        int line = Random.Range(0, CombatManager.instance.girdSize.y - 1);
        CombatManager.instance.SummonEnemy(EnemyType.Snowball2Line, line, transform.position.z);
    }

    protected override UniTask AttackLoop(CancellationToken ct){
        return UniTask.WhenAll(base.AttackLoop(ct), ThrowLoop(ct));
    }

    private async UniTask ThrowLoop(CancellationToken ct){
        while(true){
            if(data.enemy_captainpolarbear_throwInterval <= 0){
                await UniTask.Yield(cancellationToken: ct);
                continue;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(data.enemy_captainpolarbear_throwInterval), cancellationToken: ct);
            ct.ThrowIfCancellationRequested();
            ThrowSnowballs();
        }
    }

    private void ThrowSnowballs(){
        activeStrikes.RemoveAll(strike => strike == null);
        List<Slot> targetSlots = new List<Slot>(CombatManager.instance.slots);
        int throwCount = Mathf.Min(data.enemy_captainpolarbear_throwCount, targetSlots.Count);
        for(int i = 0; i < throwCount; i++){
            int targetIndex = Random.Range(0, targetSlots.Count);
            Slot targetSlot = targetSlots[targetIndex];
            targetSlots.RemoveAt(targetIndex);
            SnowballStrike strike = Instantiate(snowballStrikePrefab, CombatManager.instance.enemyBulletRoot);
            strike.StartStrike(targetSlot, data.enemy_captainpolarbear_throwDamage);
            activeStrikes.Add(strike);
        }
    }

    protected override void OnDeath_Internal(){
        CancelStrikes();
        base.OnDeath_Internal();
    }

    private void OnDestroy(){
        CancelStrikes();
    }

    private void CancelStrikes(){
        foreach(SnowballStrike strike in activeStrikes){
            if(strike != null){
                strike.Cancel();
            }
        }
        activeStrikes.Clear();
    }
}
