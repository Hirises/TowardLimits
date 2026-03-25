using System;
using UnityEngine;

public class StageModel
{
    public Vector2Int stageNumber;
    [NonSerialized] public bool isOverriden = false;
    public Polar direction;
    public int waveCount;
    public int DT;
    public int prove;

    public static StageModel FromStageData(StageData stageData){
        return new StageModel{
            stageNumber = stageData.stageNumber,
            direction = stageData.direction,
            waveCount = stageData.waveCount,
            DT = stageData.DT,
            prove = stageData.prove,
        };
    }
}