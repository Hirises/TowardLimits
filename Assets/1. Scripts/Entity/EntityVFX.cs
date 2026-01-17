using DG.Tweening;
using UnityEngine;

public class EntityVFX
{
    private LivingEntity entity;

    public void Initalize(LivingEntity entity){
        this.entity = entity;
        entity.onBeforeTakeDamage += ShowDamageVFX;
    }

    public void Dispose(){
        entity.onBeforeTakeDamage -= ShowDamageVFX;
        entity = null;
    }

    private void ShowDamageVFX(int damage){
        var inst = GameObject.Instantiate(ResourceHolder.Instance.damageVFXPrefab, entity.transform.position, Quaternion.identity);
        inst.transform.position = entity.transform.position;
        inst.Show(damage);
    }
}