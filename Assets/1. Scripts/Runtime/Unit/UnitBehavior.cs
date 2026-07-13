using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using System;

public abstract class UnitBehavior : LivingEntity
{
    public abstract UnitType unitType { get; }
    [ReadOnly] public UnitStatus status;
    [ReadOnly] public Slot slot;

    public event Action onSkillPerform;

    protected UnitVFX vfx;

    /// <summary>
    /// 유닛 생성시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(UnitStatus data){
        this.status = data;
        spriteRenderer.color = Color.white;

        vfx = new UnitVFX(this);
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
        slot.SetUnit_Internal(this);
        transform.SetParent(newSlot.UnitPoint.transform);
        transform.localPosition = Vector3.zero;
        OnPlacement_Internal();
    }

    protected abstract void OnPlacement_Internal();

    /// <summary>
    /// 유닛 회수시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public void OnDisplacement(){
        OnDisplacement_Internal();
        slot.SetUnit_Internal(null);
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
        // 드래그 중이면 드래그 취소
        CombatManager.instance.OnUnitDeath(this);
        
        OnCombatEnd();
        OnDisplacement();
        Remove();
        CombatManager.instance.CheckGameOver();
    }

    protected override int TakeDamage_Internal(int damage, DamageType type){
        int finalDamage = damage;
        if(type == DamageType.Zero){
            finalDamage = Mathf.RoundToInt(damage * (100f / (100 + status.model.zeroDEF)));
        }
        else if(type == DamageType.Infinite){
            finalDamage = Mathf.RoundToInt(damage * (100f / (100 + status.model.infDEF)));
        }
        status.currentHealth -= finalDamage;
        if(status.currentHealth <= 0){
            OnDeath();
            return finalDamage;
        }
        return finalDamage;
    }

    protected override int Heal_Internal(int amount){
        status.currentHealth += amount;
        if(status.currentHealth > status.model.maxHealth) status.currentHealth = status.model.maxHealth;
        return amount;
    }

    public void PerformSkill(){
        onSkillPerform?.Invoke();
        PerformSkill_Internal();
    }
    protected abstract void PerformSkill_Internal();

    public void OnMouseDown(){
        CombatManager.instance.StartDrag(this);
    }

    public void StartDrag(){
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    public void EndDrag(){
        spriteRenderer.color = Color.white;
    }
}
