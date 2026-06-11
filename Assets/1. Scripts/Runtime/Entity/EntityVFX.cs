using DG.Tweening;
using UnityEngine;

/// <summary>
/// 엔티티의 범용 VFX 관리용 클래스
/// </summary>
public class EntityVFX
{
    private LivingEntity entity;
    private Sequence damageEffectTween;

    public EntityVFX(LivingEntity entity){
        this.entity = entity;
        entity.onAfterTakeDamage += ShowDamageVFX;
        entity.onAfterHeal += ShowHealVFX;
    }

    public virtual void Dispose(){
        if(damageEffectTween != null){
            damageEffectTween.Kill();
        }
        damageEffectTween = null;
        if(entity != null){
            entity.onAfterTakeDamage -= ShowDamageVFX;
            entity.onAfterHeal -= ShowHealVFX;
            entity = null;
        }
    }

    private void ShowDamageVFX(int damage, DamageType type){
        var inst = GameObject.Instantiate(ResourceHolder.Instance.damageVFXPrefab, entity.transform.position, Quaternion.identity);
        inst.transform.position = entity.transform.position;
        inst.Show(damage);
        InvokeHitFade();
    }

    private void InvokeHitFade(){
        if(entity == null){
            return;
        }
        if(damageEffectTween != null){
            damageEffectTween.Kill();
        }
        damageEffectTween = DOTween.Sequence();
        damageEffectTween.Append(entity.SpriteRenderer.DOColor(Color.red, 0.07f).SetEase(Ease.OutQuad));
        damageEffectTween.Append(entity.SpriteRenderer.DOColor(Color.white, 0.07f).SetEase(Ease.OutQuad));
        damageEffectTween.Play();
    }

    private void ShowHealVFX(int heal){
        var inst = GameObject.Instantiate(ResourceHolder.Instance.healVFXPrefab, entity.transform.position, Quaternion.identity);
        inst.transform.position = entity.transform.position;
        inst.Show(heal);
    }
}