using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    protected float distance_target;
    protected float speed;
    protected int damage;
    protected DamageType damageType;


    public virtual void Shoot(BulletData data){
        this.distance_target = transform.position.z + data.distance;
        this.speed = data.speed;
        this.damage = data.damage;
        this.damageType = data.damageType;
    }

    private void Update(){
        transform.position += Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z >= distance_target){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            OnHitEnemy(other.gameObject.GetComponentInParent<EnemyBehavior>());
        }
    }

    public virtual void OnHitEnemy(EnemyBehavior enemy){
        enemy.TakeDamage(damage, damageType);
        Destroy(gameObject);
    }
}

public struct BulletData
{
    public float distance;
    public float speed;
    public DamageType damageType;
    public int damage;
}