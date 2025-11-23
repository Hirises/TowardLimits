using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    protected float distance_target;
    protected float speed;
    protected int damage;

    public virtual void Shoot(float distance, float speed, int damage){
        this.distance_target = transform.position.z + distance;
        this.speed = speed;
        this.damage = damage;
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
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
