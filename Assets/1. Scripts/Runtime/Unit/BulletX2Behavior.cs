using UnityEngine;

public class BulletX2Behavior : BulletBehavior
{
    [SerializeField] private BoxCollider centerHitbox;
    [SerializeField] private BoxCollider leftHitbox;
    [SerializeField] private BoxCollider rightHitbox;
    [SerializeField] private Vector3 hitboxSize = new Vector3(1.4f, 1f, 0.4f);
    [SerializeField, Min(0)] private float centerForwardOffset = 0.55f;

    private bool hasHit;

    public void ConfigureLines(float? leftOffset, float? rightOffset){
        centerHitbox.size = hitboxSize;
        centerHitbox.center = Vector3.forward * centerForwardOffset;
        ConfigureSideHitbox(leftHitbox, leftOffset);
        ConfigureSideHitbox(rightHitbox, rightOffset);
    }

    private void ConfigureSideHitbox(BoxCollider hitbox, float? offset){
        hitbox.enabled = offset.HasValue;
        hitbox.size = hitboxSize;
        hitbox.center = Vector3.right * offset.GetValueOrDefault();
    }

    public override void OnHitEnemy(EnemyBehavior enemy){
        if(hasHit || enemy == null) return;
        // Destroy는 프레임 끝에 적용되므로 나머지 충돌 콜백을 즉시 차단한다.
        hasHit = true;
        centerHitbox.enabled = false;
        leftHitbox.enabled = false;
        rightHitbox.enabled = false;
        speed = 0;
        base.OnHitEnemy(enemy);
    }
}
