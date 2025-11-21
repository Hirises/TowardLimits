using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private float distance_target;
    private float speed;
    private int damage;

    public void Shoot(float distance, float speed, int damage){
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
            other.gameObject.GetComponentInParent<EnemyBehavior>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
