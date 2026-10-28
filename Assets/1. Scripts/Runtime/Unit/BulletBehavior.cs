using Unity.VisualScripting;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected float distance_target;
    protected float speed;
    protected int damage;
    protected DamageType damageType;


    public virtual void Shoot(BulletData data){
        this.distance_target = transform.position.z + data.distance;
        this.speed = data.speed;
        this.damage = data.damage;
        this.damageType = data.damageType;
        spriteRenderer.color = data.color;
    }

    private void Update(){
        transform.position += Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z >= distance_target){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(!gameObject.activeInHierarchy) return;
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            OnHitEnemy(other.gameObject.GetComponentInParent<EnemyBehavior>());
        }
    }

    public virtual void OnHitEnemy(EnemyBehavior enemy){
        if(!gameObject.activeInHierarchy) return;
        TryCancel();
        enemy.TakeDamage(damage, damageType);
    }

    public bool TryCancel(){
        if(!gameObject.activeInHierarchy) return false;
        // Destroy는 프레임 끝에 적용되므로 다중 콜라이더의 남은 충돌을 즉시 차단한다.
        gameObject.SetActive(false);
        Destroy(gameObject);
        return true;
    }
}

public struct BulletData
{
    public Color color;
    public float distance;
    public float speed;
    public DamageType damageType;
    public int damage;
}
