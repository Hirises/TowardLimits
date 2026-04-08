using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

    //해당 방향 스테이지 중 랜덤으로 선택
    public void SetRandomStage(Polar direction){
        playerData.direction = direction;
        StageModel[] stageDatas = DataFetcher.stageData.Where(stage => stage.stageNumber.x <= playerData.stage 
            && stage.stageNumber.y >= playerData.stage
            && (stage.direction == playerData.direction || stage.direction == Polar.Both)).ToArray();
        currentStage = stageDatas[Random.Range(0, stageDatas.Length)];
    }
}
