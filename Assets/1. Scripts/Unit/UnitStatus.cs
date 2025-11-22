using UnityEngine;

public class UnitStatus
{
    public UnitType unitType;
    public int level;
    
    public int maxHealth;
    public int currentHealth;

    public UnitData data;

    public UnitStatus(UnitData data){
        this.data = data;
        unitType = data.unitType;
        level = 1;
        maxHealth = data.maxHealth;
        currentHealth = data.maxHealth;
    }
    
    public void ResetStatus(){
        currentHealth = maxHealth;
    }

    public static UnitStatus FromType(UnitType unitType){
        UnitData data = GameManager.instance.GetUnitData(unitType);
        return new UnitStatus(data);
    }
}
