using UnityEngine;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 인게임 메니저
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Grid")]
    [Label("그리드 크기 Row, Column")]
    [SerializeField] public Vector2Int girdSize;
    [Label("슬롯 (좌상단에서 시작. hor->ver)")]
    [SerializeField] public Slot[] slots;

    [Header("Entities")]
    [SerializeField] public SerializableDictionaryBase<EnemyType, EnemyBehavior> enemyMap;
    [SerializeField] public SerializableDictionaryBase<UnitType, UnitBehavior> unitMap;
    [SerializeField] public Transform enemySpawnRoot;

    private List<EnemyBehavior> enemiesList = new List<EnemyBehavior>();
    private Coroutine enemySpawnLoop;

    [ReadOnly] public Phase phase;

    public enum Phase {
        None,       //초기 상태
        Placement,  //배치 단계
        Combat,     //전투 단계
    }


#region UnityLifeCycle
    private void Awake(){
        instance = this;
    }

    private void Start(){
        SetupGame();
        StartGame();
    }

    private void Update(){
        if(phase == Phase.Placement){
            if(Input.GetKeyDown(KeyCode.Space)){
                StartCombatPhase();
            }
        }else if(phase == Phase.Combat){
            if(Input.GetKeyDown(KeyCode.Space)){
                StartPlacementPhase();
            }
        }
    }
#endregion

#region Game Management
    /// <summary>
    /// 게임을 종료하고 자원을 반환합니다.
    /// </summary>
    public void EndGame(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                Destroy(slot.unit.gameObject);
            }
            slot.unit = null;
        }
        foreach(EnemyBehavior enemy in enemiesList){
            Destroy(enemy.gameObject);
        }
        enemiesList.Clear();
        phase = Phase.None;
        StopCoroutine(enemySpawnLoop);
        enemySpawnLoop = null;
    }

    /// <summary>
    /// 게임을 초기화하고 준비합니다 (이미 모든 자원은 반환되어있는 빈 상태라고 가정)
    /// </summary>
    public void SetupGame(){
        PlayerData playerData = GameManager.instance.playerData;

        //배치자료대로 유닛 생성
        foreach(Slot slot in slots){
            UnitData unit = playerData.units[slot.position.x][slot.position.y];
            if(unit.unitType == UnitType.None){
                continue;
            }

            UnitBehavior unitInstance = Instantiate(unitMap[unit.unitType], slot.transform);
            slot.unit = unitInstance;
            unitInstance.Initialize(unit);
            unitInstance.OnPlacement();
        }
        
    }

    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame(){
        StartPlacementPhase();
    }

    public void StartPlacementPhase(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatEnd();
            }
        }
        phase = Phase.Placement;
        if(enemySpawnLoop != null) StopCoroutine(enemySpawnLoop);
        enemySpawnLoop = null;
    }

    public void StartCombatPhase(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatStart();
            }
        }
        phase = Phase.Combat;
        enemySpawnLoop = StartCoroutine(SimpleEnemySpawnLoop());
    }
#endregion

    private IEnumerator SimpleEnemySpawnLoop(){
        while(true){
            yield return new WaitForSeconds(0.1f);
            SummonEnemy(EnemyType.EIRNormal, Random.Range(0, girdSize.y));
        }
    }

    public void SummonEnemy(EnemyType enemyType, int column){
        EnemyBehavior enemy = Instantiate(enemyMap[enemyType], enemySpawnRoot);
        enemy.transform.position = new Vector3(RelavtiveLineHandler.instance.ColumnX(column), 0, RelavtiveLineHandler.instance.TopRowZ);
        enemiesList.Add(enemy);
        enemy.OnSummon();
    }

    public Slot GetSlot(int row, int column){
        return slots[row * girdSize.y + column];
    }
}
