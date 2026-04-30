using System;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    protected float speed;
    protected int damage;

    public virtual void Shoot(float speed, int damage){
        this.speed = speed;
        this.damage = damage;
    }

    private void Update(){
        transform.position -= Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z <= RelavtiveLineHandler.instance.BottomRowZ){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log($"EnemyBullet hit {other.gameObject.name}");
        if(other.gameObject.layer == LayerMask.NameToLayer("Unit")){
            OnHitUnit(other.gameObject.GetComponentInParent<UnitBehavior>());
        }
    }

    public virtual void OnHitUnit(UnitBehavior unit){
        unit.TakeDamage(damage);
    }
}