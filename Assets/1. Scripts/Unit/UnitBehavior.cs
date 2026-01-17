using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public abstract class UnitBehavior : LivingEntity
{
    public abstract UnitType unitType { get; }
    [ReadOnly] public UnitStatus status;
    [SerializeField] private MeshRenderer meshRenderer;
    [ReadOnly] public Slot slot;

    private EntityVFX vfx;

    /// <summary>
    /// 유닛 생성시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(UnitStatus data){
        this.status = data;
        meshRenderer.material.color = Color.white;

        vfx = new EntityVFX();
        vfx.Initalize(this);
    }

    /// <summary>
    /// 유닛 제거시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public void Remove(){
        status = null;
        vfx.Dispose();
        Destroy(gameObject);
    }

    /// <summary>
    /// 유닛 설치시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public void OnPlacement(Slot newSlot){
        slot = newSlot;
        slot.unit = this;
        transform.SetParent(newSlot.transform);
        transform.localPosition = Vector3.zero;
        OnPlacement_Internal();
    }

    protected abstract void OnPlacement_Internal();

    /// <summary>
    /// 유닛 회수시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public void OnDisplacement(){
        OnDisplacement_Internal();
        slot.unit = null;
        slot = null;
        transform.SetParent(null);
    }

    protected abstract void OnDisplacement_Internal();

    /// <summary>
    /// 전투 시작시
    /// </summary>
    public void OnCombatStart(){
        OnCombatStart_Internal();
    }

    protected abstract void OnCombatStart_Internal();

    /// <summary>
    /// 전투 종료시
    /// </summary>
    public abstract void OnCombatEnd();

    /// <summary>
    /// 유닛 사망시
    /// </summary>
    public void OnDeath(){
        OnCombatEnd();
        OnDisplacement();
        Remove();
    }

    protected override void TakeDamage_Internal(int damage){
        status.currentHealth -= damage;
        if(status.currentHealth <= 0){
            OnDeath();
        }
    }

    public void OnMouseDown(){
        CombatManager.instance.StartDrag(this);
    }

    public void StartDrag(){
        meshRenderer.material.color = new Color(1, 0, 0, 0.5f);
    }

    public void EndDrag(){
        meshRenderer.material.color = Color.white;
    }
}
