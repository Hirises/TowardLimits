using UnityEngine;

/// <summary>
/// 유틸 확장 메소드
/// </summary>
public static class StaticUtil
{
    public static UnitModel GetUnitModel(this UnitType unitType){
        return DataFetcher.GetUnitModel(unitType);
    }

    public static EnemyModel GetEnemyModel(this EnemyType enemyType){
        return DataFetcher.GetEnemyModel(enemyType);
    }
}
