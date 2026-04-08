using DG.Tweening;
using UnityEngine;

/// <summary>
/// 적군 VFX 처리용 클래스
/// 당장은 아무런 추가 기능이 없다
/// </summary>
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