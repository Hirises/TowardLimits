using System;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    protected float speed;
    protected int damage;
    private int remainingCancelCount;

    public virtual void Shoot(float speed, int damage, int cancelCount){
        this.speed = speed;
        this.damage = damage;
        remainingCancelCount = Mathf.Max(1, cancelCount);
    }

    private void Update(){
        transform.position -= Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z <= RelavtiveLineHandler.instance.BottomRowZ){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(!gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit_Bullet")){
            BulletBehavior bullet = other.GetComponentInParent<BulletBehavior>();
            if(bullet.TryCancel()){
                remainingCancelCount--;
                if(remainingCancelCount <= 0){
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
            return;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit")){
            OnHitUnit(other.gameObject.GetComponentInParent<UnitBehavior>());
        }
    }

    public virtual void OnHitUnit(UnitBehavior unit){
        if(!gameObject.activeInHierarchy) return;
        gameObject.SetActive(false);
        unit.TakeDamage(damage, DamageType.None);
        Destroy(gameObject);
    }
}
