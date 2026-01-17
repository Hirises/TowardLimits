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

    [Header("Data")]
    [SerializeField] public ResourceHolder resourceHolder;

    [Header("Objects")]
    [SerializeField] public StageData[] stageDatas;
    [SerializeField] public UnitData[] unitDatas;
    [SerializeField] public EnemyData[] enemyDatas;
    [SerializeField] public List<UnitType> initalUnitlist;

    [Header("Debug")]
    [SerializeField] public bool DEBUG_MODE = false;
    [SerializeField] public bool SKIP_INTENTIONAL_DELAY = false;
    [SerializeField] public bool SKIP_CUTSCENE = false;
    [SerializeField] public float MIN_LOADING_DELAY = 3f;
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

        if(playerData == null){
            playerData = new PlayerData();
        }
        if(DEBUG_MODE){
            playerData.units = new List<UnitStatus>();
            foreach(UnitType unitType in unitlist){
                if(unitType == UnitType.None){
                    continue;
                }
                playerData.units.Add(UnitStatus.FromType(unitType));
            }
            playerData.direction = direction;
        }
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
