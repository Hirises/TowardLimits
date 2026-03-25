using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    [SerializeField, InlineProperty, HideLabel] public CommonSettingsModel commonSettings;

    [Header("Debug")]
    [SerializeField] public bool DEBUG_MODE = false;
    [SerializeField] public bool SKIP_INTENTIONAL_DELAY = false;
    [SerializeField] public bool SKIP_CUTSCENE = false;
    [SerializeField] public float MIN_LOADING_DELAY = 3f;
    [SerializeField] private UnitType[] unitlist;
    [SerializeField] private Polar direction;

    public PlayerData playerData;
    public StageModel currentStage;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        //싱글톤 검증 완료 -----

        DataFetcher.FetchData();
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
        Application.targetFrameRate = 60;
    }
}
