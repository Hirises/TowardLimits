using UnityEngine;

public class EIRNormal : EnemyBehavior
{
    public override EnemyType enemyType => EnemyType.EIRNormal;

    [SerializeField] private float moveSpeed;

    private void Start(){
        OnSummon();
    }

    protected override void OnSummon_Internal(){
    }

    public override void OnDeath(){
        Destroy(gameObject);
    }

    private void Update(){
        transform.position -= Vector3.forward * moveSpeed * Time.deltaTime;
        if(transform.position.z <= -10){
            OnDeath();
        }
    }
}
