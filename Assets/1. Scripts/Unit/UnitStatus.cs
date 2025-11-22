using UnityEngine;

public class UnitStatus
{
    public UnitType unitType;
    public int level;
    
    public int maxHealth;
    public int currentHealth;
    
    public void ResetStatus(){
        currentHealth = maxHealth;
    }

    public UnitStatus FromType(UnitType unitType){
        UnitData data = GameManager.instance.GetUnitData(unitType);
        return new UnitStatus{ unitType = unitType, level = 1, maxHealth = data.maxHealth, currentHealth = data.maxHealth };
    }
}
