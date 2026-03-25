using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 인게임 데이터 파싱 클래스
/// </summary>
public static class DataFetcher
{
    private const string COMMON_DATA_PATH = "OverrideData/common_settings.json";
    private const string STAGE_DATA_PATH = "OverrideData/Stage";
    private const string UNIT_DATA_PATH = "OverrideData/Unit";
    private const string ENEMY_DATA_PATH = "OverrideData/Enemy";
    private const string WAVE_DATA_PATH = "OverrideData/Wave";
    private const string WAVE_ADDITIONAL_DATA_PATH = "OverrideData/WaveAdditional";
    private const string STAGE_ADDITIONAL_DATA_PATH = "OverrideData/StageAdditional";

    private static bool isInitialized = false;

    public static StageModel[] stageData;
    public static WaveModel[] waveData;
    public static Dictionary<UnitType, UnitModel> unitData;
    public static Dictionary<EnemyType, EnemyModel> enemyData;

    public static UnitModel GetUnitModel(UnitType unitType){
        if(!isInitialized){
            FetchData();
        }
        return unitData[unitType];
    }

    public static EnemyModel GetEnemyModel(EnemyType enemyType){
        if(!isInitialized){
            FetchData();
        }
        return enemyData[enemyType];
    }

#region Fetch Data
    public static void FetchData(){
        FetchCommonSettings();
        stageData = FetchStageData();
        waveData = FetchWaveData();
        unitData = FetchUnitData();
        enemyData = FetchEnemyData();
        
        isInitialized = true;
    }
    
    public static void FetchCommonSettings(){
        #if OVERRIDE_DATA
        string filePath = Path.Combine(Application.persistentDataPath, COMMON_DATA_PATH);
        if(File.Exists(filePath)){
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, GameManager.instance.commonSettings);
        }
        #endif
    }
    public static Dictionary<EnemyType, EnemyModel> FetchEnemyData(){
        Dictionary<EnemyType, EnemyModel> enemyModels = new Dictionary<EnemyType, EnemyModel>();
        foreach(EnemyData enemyData in ResourceHolder.Instance.enemyDatas){
            enemyModels.Add(enemyData.enemyModel.enemyType, enemyData.enemyModel.Clone() as EnemyModel);
        }
        
        #if OVERRIDE_DATA
        string filePath = Path.Combine(Application.persistentDataPath, ENEMY_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                if(Enum.TryParse<EnemyType>(Path.GetFileNameWithoutExtension(file), out EnemyType enemyType)){
                    Debug.Log($"Override EnemyData: {enemyType}");
                    string json = File.ReadAllText(file);
                    JsonUtility.FromJsonOverwrite(json, enemyModels[enemyType]);
                    enemyModels[enemyType].isOverriden = true;
                }
            }
        }
        #endif
        
        return enemyModels;
    }
    
    public static Dictionary<UnitType, UnitModel> FetchUnitData(){
        Dictionary<UnitType, UnitModel> unitModels = new Dictionary<UnitType, UnitModel>();
        foreach(UnitData unitData in ResourceHolder.Instance.unitDatas){
            unitModels.Add(unitData.unitModel.unitType, unitData.unitModel.Clone() as UnitModel);
        }
        
        #if OVERRIDE_DATA
        string filePath = Path.Combine(Application.persistentDataPath, UNIT_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                if(Enum.TryParse<UnitType>(Path.GetFileNameWithoutExtension(file), out UnitType unitType)){
                    Debug.Log($"Override UnitData: {unitType}");
                    string json = File.ReadAllText(file);
                    JsonUtility.FromJsonOverwrite(json, unitModels[unitType]);
                    unitModels[unitType].isOverriden = true;
                }
            }
        }
        #endif
        
        return unitModels;
    }
    
    public static WaveModel[] FetchWaveData(){
        List<WaveModel> waveModels = new List<WaveModel>();
        
        #if OVERRIDE_DATA
        string filePath = Path.Combine(Application.persistentDataPath, WAVE_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                string json = File.ReadAllText(file);
                WaveModel waveModel = JsonUtility.FromJson<WaveModel>(json);
                waveModels.Add(waveModel);
                waveModel.isOverriden = true;
            }
            Debug.Log($"Override WaveData: {waveModels.Count}");
            return waveModels.ToArray();
        }

        filePath = Path.Combine(Application.persistentDataPath, WAVE_ADDITIONAL_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                string json = File.ReadAllText(file);
                WaveModel waveModel = JsonUtility.FromJson<WaveModel>(json);
                waveModels.Add(waveModel);
                waveModel.isOverriden = true;
            }
            Debug.Log($"Override WaveData: {waveModels.Count}");
        }
        #endif
    
        foreach(WaveChart waveChart in ResourceHolder.Instance.waveCharts){
            waveModels.Add(WaveModel.FromWaveChart(waveChart));
        }
        return waveModels.ToArray();
    }

    public static StageModel[] FetchStageData(){
        List<StageModel> stageModels = new List<StageModel>();
        
        #if OVERRIDE_DATA
        string filePath = Path.Combine(Application.persistentDataPath, STAGE_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                string json = File.ReadAllText(file);
                StageModel stageModel = JsonUtility.FromJson<StageModel>(json);
                stageModels.Add(stageModel);
                stageModel.isOverriden = true;
            }
            Debug.Log($"Override StageData: {stageModels.Count}");
            return stageModels.ToArray();
        }
        
        filePath = Path.Combine(Application.persistentDataPath, STAGE_ADDITIONAL_DATA_PATH);
        if(Directory.Exists(filePath)){
            //override
            foreach(string file in Directory.EnumerateFiles(filePath, "*.json", SearchOption.TopDirectoryOnly)){
                string json = File.ReadAllText(file);
                StageModel stageModel = JsonUtility.FromJson<StageModel>(json);
                stageModels.Add(stageModel);
                stageModel.isOverriden = true;
            }
            Debug.Log($"Override StageData: {stageModels.Count}");
        }
        #endif
    
        foreach(StageData stageData in ResourceHolder.Instance.stageData){
            stageModels.Add(StageModel.FromStageData(stageData));
        }
        return stageModels.ToArray();
    }
#endregion
}
