using System.Collections.Generic;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

/// <summary>
/// 전체 씬 홀더
/// </summary>
public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager instance;

    [Header("Objects")]
    [SerializeField] public StageData[] stageDatas;
    [SerializeField] public UnitData[] unitDatas;
    [SerializeField] public EnemyData[] enemyDatas;

    [Header("Debug")]
    [SerializeField] private UnitType[] unitlist;
    [SerializeField] private Polar direction;

    public PlayerData playerData;
    public StageData currentStage;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        playerData = new PlayerData();
        playerData.units = new List<UnitStatus>();
        foreach(UnitType unitType in unitlist){
            if(unitType == UnitType.None){
                continue;
            }
            playerData.units.Add(UnitStatus.FromType(unitType));
        }
        playerData.direction = direction;
    }

    public UnitData GetUnitData(UnitType unitType){
        foreach(UnitData data in unitDatas){
            if(data.unitType == unitType){
                return data;
            }
        }
        return null;
    }

    public EnemyData GetEnemyData(EnemyType enemyType){
        foreach(EnemyData data in enemyDatas){
            if(data.enemyType == enemyType){
                return data;
            }
        }
        return null;
    }
}
