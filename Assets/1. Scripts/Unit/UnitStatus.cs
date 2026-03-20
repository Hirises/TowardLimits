using Unity.VisualScripting;
using UnityEngine;

public class UnitStatus
{
    public UnitType unitType;
    public int level;
    
    public int maxHealth;
    public int currentHealth;

    public int CurrentAttack => model.attack + (level - 1);
    public int CurrentMaxHealth => model.maxHealth + 5 * (level - 1);

    public UnitModel model;

    public UnitStatus(UnitModel model){
        this.model = model;
        unitType = model.unitType;
        level = 1;
        maxHealth = CurrentMaxHealth;
        currentHealth = CurrentMaxHealth;
    }
    
    public void ResetStatus(){
        currentHealth = maxHealth;
    }

    public static UnitStatus FromType(UnitType unitType){
        UnitModel model = DataFetcher.GetUnitModel(unitType);
        return new UnitStatus(model);
    }
}
