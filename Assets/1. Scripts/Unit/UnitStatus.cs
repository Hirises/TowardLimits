using Unity.VisualScripting;
using UnityEngine;

public class UnitStatus
{
    public UnitType unitType;
    public int level;
    
    public int maxHealth;
    public int currentHealth;

    public int CurrentAttack => data.attack + (level - 1);
    public int CurrentMaxHealth => data.maxHealth + 5 * (level - 1);

    public UnitData data;

    public UnitStatus(UnitData data){
        this.data = data;
        unitType = data.unitType;
        level = 1;
        maxHealth = CurrentMaxHealth;
        currentHealth = CurrentMaxHealth;
    }
    
    public void ResetStatus(){
        currentHealth = maxHealth;
    }

    public static UnitStatus FromType(UnitType unitType){
        UnitData data = GameManager.instance.GetUnitData(unitType);
        return new UnitStatus(data);
    }
}
