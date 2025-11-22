using UnityEngine;

public static class StaticUtil
{
    public static UnitData GetUnitData(this UnitType unitType){
        return GameManager.instance.GetUnitData(unitType);
    }

    public static EnemyData GetEnemyData(this EnemyType enemyType){
        return GameManager.instance.GetEnemyData(enemyType);
    }
}
