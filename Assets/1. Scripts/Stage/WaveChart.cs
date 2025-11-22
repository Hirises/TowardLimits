using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveChart.asset", menuName = "Wave Chart")]
public class WaveChart : ScriptableObject
{
    public Polar polar = Polar.Both; //북쪽방향인가?
    public Vector2Int difficulty;   //난이도 계수

    public string filePath;   //적 생성용 파일경로
    public WaveChartList waveChartList;
    [System.NonSerialized] public List<(float startTime, int lane, EnemyType enemyType)> summonList;

    public void Load(){
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if(textAsset == null){
            Debug.LogError($"WaveChart: {filePath} not found");
            return;
        }
        string json = textAsset.text;
        waveChartList = JsonUtility.FromJson<WaveChartList>(json);
        summonList = new List<(float startTime, int lane, EnemyType enemyType)>();
        foreach(WaveChartData data in waveChartList.enemyList){
            if(data.emitOnce){
                summonList.Add((data.startTime, data.lane, data.enemyType));
            }else{
                for(int i = 0; i < data.count; i++){
                    summonList.Add((data.startTime + i * data.interval, data.lane, data.enemyType));
                }
            }
        }
        summonList.Sort((a, b) => a.startTime.CompareTo(b.startTime));
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
        public EnemyType enemyType;
    }
}
