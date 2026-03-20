using UnityEngine;

public static class StaticUtil
{
    public static UnitModel GetUnitModel(this UnitType unitType){
        return DataFetcher.GetUnitModel(unitType);
    }

    public static EnemyData GetEnemyData(this EnemyType enemyType){
        return GameManager.instance.GetEnemyData(enemyType);
    }
}
