using DG.Tweening;
using UnityEngine;

public class EnemyVFX : EntityVFX {
    private EnemyBehavior enemy;

    public EnemyVFX(EnemyBehavior enemy) : base(enemy){
        this.enemy = enemy;
    }

    public override void Dispose(){
        if(enemy == null){
            return;
        }
        enemy = null;
        base.Dispose();
    }
}