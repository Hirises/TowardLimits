using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "WaveChart.asset", menuName = "Wave Chart")]
public class WaveChart : ScriptableObject
{
    public Polar polar = Polar.Both; //북쪽방향인가?
    public Vector2Int stageRange;   //스테이지
    public Vector2Int difficulty;   //난이도 계수
    public bool forFinalBoss = false;   //마지막 보스 전용 차트인가?

    public List<WaveChartData> enemyList;

    #if UNITY_EDITOR
    //그냥 enemyList 자체를 직렬화해서 저장하는 방식으로 변경함. 아래는 예전 방식으로 저장된 파일을 파싱하기 위한 용도
    public string filePath;   //적 생성용 파일경로
    [Button]
    public void LoadFromFile(){
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if(textAsset == null){
            Debug.LogError($"WaveChart: {filePath} not found");
            return;
        }
        string json = textAsset.text;
        JsonUtility.FromJsonOverwrite(json, this);
        foreach(WaveChartData data in enemyList){
            Debug.Log($"WaveChartData: {data.enemyType_enum} at {data.lane} at {data.startTime}");
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log($"WaveChart: {name} loaded");
    }
    #endif
}
