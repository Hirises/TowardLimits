using UnityEngine;

[CreateAssetMenu(fileName = "StageData.asset", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    public int difficulty;
    public int waveCount;
    public int DT;
}
