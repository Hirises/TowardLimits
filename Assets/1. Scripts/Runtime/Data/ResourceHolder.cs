using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "ResourceHolder.asset", menuName = "Unique/Resource Holder")]
public class ResourceHolder : ScriptableObject
{
    private static ResourceHolder instance;
    public static ResourceHolder Instance
    {
        get {
            if(instance == null){
                if(GameManager.instance == null || GameManager.instance.resourceHolder == null){
                    instance = Resources.Load<ResourceHolder>("ResourceHolder");
                }else{
                    instance = GameManager.instance.resourceHolder;
                }
            }
            return instance;
        }
    }

    [Header("Data")]
    [SerializeField] public StageData[] stageData;
    [SerializeField] public UnitData[] unitDatas;
    [SerializeField] public EnemyData[] enemyDatas;
    [SerializeField] public WaveChart[] waveCharts;
    
    [Header("VFX")]
    public DamageVFX damageVFXPrefab;
}
