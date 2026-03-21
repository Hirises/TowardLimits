using UnityEngine;

public static class StaticUtil
{
    public static UnitModel GetUnitModel(this UnitType unitType){
        return DataFetcher.GetUnitModel(unitType);
    }

    public static EnemyModel GetEnemyModel(this EnemyType enemyType){
        return DataFetcher.GetEnemyModel(enemyType);
    }
}
