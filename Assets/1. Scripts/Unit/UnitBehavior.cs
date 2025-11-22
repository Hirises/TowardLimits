using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;

public abstract class UnitBehavior : MonoBehaviour
{
    public abstract UnitType unitType { get; }
    [ReadOnly] public UnitData data;

    public void Initialize(UnitData data){
        this.data = data;
    }

    public void Clear(){
        data = null;
    }

    /// <summary>
    /// 유닛 설치시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public abstract void OnPlacement();

    /// <summary>
    /// 유닛 회수시 (항상 전투가 종료된 상태라고 가정)
    /// </summary>
    public abstract void OnDisplacement();

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
    }

    public void TakeDamage(int damage){
        data.currentHealth -= damage;
        if(data.currentHealth <= 0){
            OnDeath();
        }
    }

    public void OnMouseDrag(){
        Debug.Log($"{unitType} OnMouseDrag");
    }
}
