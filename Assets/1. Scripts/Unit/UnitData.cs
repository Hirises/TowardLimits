using UnityEngine;

public class UnitData
{
    public UnitType unitType;
    public int level;
    
    public int maxHealth;
    public int currentHealth;
    
    public void ResetStatus(){
        currentHealth = maxHealth;
    }
}
