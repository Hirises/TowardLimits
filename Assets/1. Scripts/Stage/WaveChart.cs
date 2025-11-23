using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveChart.asset", menuName = "Wave Chart")]
public class WaveChart : ScriptableObject
{
    public Polar polar = Polar.Both; //북쪽방향인가?
    public Vector2Int difficulty;   //난이도 계수
    public bool forFinalBoss = false;   //마지막 보스 전용 차트인가?

    public string filePath;   //적 생성용 파일경로
    [HideInInspector] public WaveChartList waveChartList;
    [System.NonSerialized] public List<(float startTime, int lane, EnemyType enemyType)> summonList;
    [HideInInspector] public float duration;
    public EnemyType[][] commonEnemyTypes; //경고해야할 적 종류. column, priority

    [Button]
    public void Load(){
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if(textAsset == null){
            Debug.LogError($"WaveChart: {filePath} not found");
            return;
        }
        string json = textAsset.text;
        waveChartList = JsonUtility.FromJson<WaveChartList>(json);
        foreach(WaveChartData data in waveChartList.enemyList){
            Debug.Log($"WaveChartData: {GetEnemyType(data.enemyType)} at {data.lane} at {data.startTime}");
        }
        summonList = new List<(float startTime, int lane, EnemyType enemyType)>();
        float maxStartTime = 0;
        foreach(WaveChartData data in waveChartList.enemyList){
            if(data.emitOnce){
                summonList.Add((data.startTime, data.lane, GetEnemyType(data.enemyType)));
                maxStartTime = Mathf.Max(maxStartTime, data.startTime);
            }else{
                for(int i = 0; i < data.count; i++){
                    summonList.Add((data.startTime + i * data.interval, data.lane, GetEnemyType(data.enemyType)));
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
        duration = 0;
        waveChartList = null;
        Debug.Log($"WaveChart: {name} unloaded");
    }

    private EnemyType GetEnemyType(string enemyType){
        return Enum.Parse<EnemyType>(enemyType);
    }

    [System.Serializable]
    public class WaveChartList{
        public List<WaveChartData> enemyList;    //생성 정보
    }

    [System.Serializable]
    public class WaveChartData{
        public int lane;
        public bool emitOnce;
        public float startTime;
        public int count;
        public float interval;
        public string enemyType;
    }
}
