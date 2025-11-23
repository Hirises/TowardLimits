using UnityEngine;

[CreateAssetMenu(fileName = "StageData.asset", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    public Vector2Int stageNumber;
    public Polar direction;
    public int waveCount;
    public int DT;
    public int prove;
}
