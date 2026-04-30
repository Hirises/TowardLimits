using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveModel
{
    public Polar polar = Polar.Both; //북쪽방향인가?
    public Vector2Int stageRange;   //스테이지
    public Vector2Int difficulty;   //난이도 계수
    public bool forFinalBoss = false;   //마지막 보스 전용 차트인가?
    [NonSerialized] public bool isOverriden = false;

    [HideInInspector] public List<WaveChartData> enemyList;    //생성 정보

    //동적 로드하는 정보
    [System.NonSerialized] public List<(float startTime, int lane, EnemyType enemyType)> summonList;
    [System.NonSerialized] public EnemyType[][] commonEnemyTypes; //경고해야할 적 종류. column, priority
    [System.NonSerialized] public float duration;

    public static WaveModel FromWaveChart(WaveChart waveChart)
    {
        var inst = new WaveModel{
            polar = waveChart.polar,
            stageRange = waveChart.stageRange,
            difficulty = waveChart.difficulty,
            forFinalBoss = waveChart.forFinalBoss,
            enemyList = waveChart.enemyList,
        };
        return inst;
    }

    [Button]
    public void Load(){
        summonList = new List<(float startTime, int lane, EnemyType enemyType)>();
        float maxStartTime = 0;
        foreach(WaveChartData data in enemyList){
            if(data.emitOnce){
                summonList.Add((data.startTime, data.lane, data.enemyType_enum));
                maxStartTime = Mathf.Max(maxStartTime, data.startTime);
            }else{
                for(int i = 0; i < data.count; i++){
                    summonList.Add((data.startTime + i * data.interval, data.lane, data.enemyType_enum));
                }
                maxStartTime = Mathf.Max(maxStartTime, data.startTime + data.count * data.interval);
            }
        }
        summonList.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        duration = maxStartTime + 15;
        commonEnemyTypes = new EnemyType[CombatManager.instance.girdSize.y][];
        for(int i = 0; i < CombatManager.instance.girdSize.y; i++){
            //find first 3 frequent enemy types
            commonEnemyTypes[i] = summonList.Where(x => x.lane == i).GroupBy(x => x.enemyType).OrderByDescending(x => x.Count()).Take(3).Select(x => x.Key).ToArray();
        }
    }

    [Button]
    public void Unload(){
        summonList = null;
        commonEnemyTypes = null;
        duration = 0;
    }
}

[System.Serializable]
public class WaveChartData : ISerializationCallbackReceiver 
{
    public int lane;
    public bool emitOnce;
    public float startTime;
    public int count;
    public float interval;
    public EnemyType enemyType_enum;
    [HideInInspector] public string enemyType;

    public void OnAfterDeserialize()
    {
        if(Enum.TryParse<EnemyType>(enemyType, out EnemyType enemyType_enum)){
            this.enemyType_enum = enemyType_enum;
        }
    }

    public void OnBeforeSerialize()
    {
        enemyType = enemyType_enum.ToString();
    }
}