using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 이차함수
/// </summary>
public class UnitX2 : DefaultUnitBehavior
{
    [SerializeField] private BulletX2Behavior skillBulletPrefab;

    public override UnitType unitType => UnitType.UnitX2;

    public override void OnShoot()
    {
        ShootTask().Forget();
    }

    private async UniTask ShootTask()
    {
        ShootBullet(slot.position.y);
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: this.GetCancellationTokenOnDestroy());
        if(slot.position.y + 1 < CombatManager.instance.girdSize.y) ShootBullet(slot.position.y + 1, status.model.unit_x2_damageRatio, 0.7f);
        if(slot.position.y - 1 >= 0) ShootBullet(slot.position.y - 1, status.model.unit_x2_damageRatio, 0.7f);
    }

    protected override UniTask SkillLoop(CancellationToken ct){
        ct.ThrowIfCancellationRequested();
        int column = slot.position.y;
        float centerX = RelavtiveLineHandler.instance.ColumnX(column);
        Vector3 position = new Vector3(centerX, bulletSpawnPoint.position.y, bulletSpawnPoint.position.z);
        BulletX2Behavior bullet = Instantiate(skillBulletPrefab, position, Quaternion.identity, CombatManager.instance.bulletRoot);
        float? leftOffset = column > 0 ? RelavtiveLineHandler.instance.ColumnX(column - 1) - centerX : (float?)null;
        float? rightOffset = column + 1 < CombatManager.instance.girdSize.y ? RelavtiveLineHandler.instance.ColumnX(column + 1) - centerX : (float?)null;
        bullet.ConfigureLines(leftOffset, rightOffset);
        bullet.Shoot(new BulletData()
        {
            distance = status.model.attackRange,
            speed = status.model.unit_x2_skillBulletSpeed,
            color = status.model.unitColor,
            damage = status.model.unit_x2_skillDamage,
            damageType = status.model.attackType
        });
        return UniTask.CompletedTask;
    }
}
