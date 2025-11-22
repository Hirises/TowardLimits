using UnityEngine;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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
    [SerializeField] public WaveChart[] waveCharts;

    [Header("UI")]
    [SerializeField] public PlacementUI placementUIRoot;

    [ReadOnly] private StageData currentStage;
    [ReadOnly] private int currentWave = 0;
    private List<EnemyBehavior> enemiesList = new List<EnemyBehavior>();
    private Coroutine enemySpawnLoop;
    private UnitBehavior draggingUnit;

    [ReadOnly] public Phase phase;
    private bool isSummonEnded = false;

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
        TestKey();

        if(phase == Phase.Placement){
            if(Input.GetKeyDown(KeyCode.Space)){
                StartCombatPhase();
            }
        }else if(phase == Phase.Combat){
            if(isSummonEnded && enemiesList.Count == 0){
                NextWave();
            }
        }

        if(draggingUnit != null){
            if(Input.GetMouseButtonUp(0)){
                EndDrag();
            }
        }
    }

    private void TestKey(){
        if(Input.GetKeyDown(KeyCode.Q)){
            Slot slot = RaycastSlot(Input.mousePosition);
            Debug.Log($"TestRaycast: {slot}");
        }
        if(phase == Phase.Combat && Input.GetKeyDown(KeyCode.Space)){
            NextWave();
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
        EndDrag();
    }

    /// <summary>
    /// 게임을 초기화하고 준비합니다 (이미 모든 자원은 반환되어있는 빈 상태라고 가정)
    /// </summary>
    public void SetupGame(){
        placementUIRoot.Hide();
        currentStage = GameManager.instance.currentStage;
        currentWave = 0;
        GameManager.instance.playerData.DT = currentStage.DT;
    }

    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame(){
        StartPlacementPhase();
    }

    public void NextWave(){
        isSummonEnded = false;
        currentWave++;
        if(currentWave >= currentStage.waveCount){
            EndGame();
            return;
        }
        StartPlacementPhase();
    }
#endregion

#region Placement Phase
    public void StartPlacementPhase(){
        if(enemySpawnLoop != null) StopCoroutine(enemySpawnLoop);
        enemySpawnLoop = null;
        isSummonEnded = false;
        EndDrag();
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatEnd();
            }
            slot.ShowBase(true);
        }
        phase = Phase.Placement;
        placementUIRoot.Show();
    }

    public Slot RaycastSlot(Vector3 screenPosition){
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Slot slot = null;
        if(Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Slot", "Unit"))){
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Slot")){
                slot = hit.collider.gameObject.GetComponentInParent<Slot>();
            }else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit")){
                UnitBehavior unit = hit.collider.gameObject.GetComponentInParent<UnitBehavior>();
                slot = unit.slot;
            }
        }
        return slot;
    }

    public void StartDrag(UnitBehavior unit){
        if(phase != Phase.Placement){
            return;
        }
        draggingUnit = unit;
        unit.StartDrag();
        Debug.Log($"StartDrag: {unit.unitType}");
    }

    public void EndDrag(){
        if(draggingUnit == null){
            return;
        }
        draggingUnit.EndDrag();
        Slot slot = RaycastSlot(Input.mousePosition);
        if(slot != null && slot.unit == null){
            draggingUnit.OnDisplacement();
            draggingUnit.OnPlacement(slot);
        }
        draggingUnit = null;
        Debug.Log($"EndDrag {slot}");
    }
#endregion

#region Combat Phase
    public void StartCombatPhase(){
        EndDrag();
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatStart();
            }
            slot.ShowBase(false);
        }
        phase = Phase.Combat;
        isSummonEnded = false;
        WaveChart waveChart = ChooseRandomWaveChart(1, Polar.Both);
        if(waveChart == null){
            enemySpawnLoop = StartCoroutine(SimpleEnemySpawnLoop());
            return;
        }
        Debug.Log($"ChooseWaveChart: {waveChart.filePath}");
        waveChart.Load();
        enemySpawnLoop = StartCoroutine(WaveChartSpawnLoop(waveChart));
        placementUIRoot.Hide();
    }

    public WaveChart ChooseRandomWaveChart(int difficulty, Polar polar){
        WaveChart[] waveChart = waveCharts.Where(w => w.difficulty.x <= difficulty && difficulty <= w.difficulty.y  
            && (w.polar == polar || w.polar == Polar.Both)).ToArray();
        if(waveChart.Length == 0){
            return null;
        }
        return waveChart[Random.Range(0, waveChart.Length)];
    }
    
    private IEnumerator WaveChartSpawnLoop(WaveChart waveChart){
        float time = 0;
        foreach(var summon in waveChart.summonList){
            if(summon.startTime - time > 0.01f){
                yield return new WaitForSeconds(summon.startTime - time);
            }
            time = summon.startTime;
            SummonEnemy(summon.enemyType, summon.lane);
        }
        isSummonEnded = true;
    }

    private IEnumerator SimpleEnemySpawnLoop(){
        for(int i = 0; i < 1000; i++){
            yield return new WaitForSeconds(0.1f);
            SummonEnemy(EnemyType.EIRNormal, Random.Range(0, girdSize.y));
        }
        isSummonEnded = true;
    }

    public void SummonEnemy(EnemyType enemyType, int column){
        EnemyBehavior enemy = Instantiate(enemyMap[enemyType], enemySpawnRoot);
        enemy.transform.position = new Vector3(RelavtiveLineHandler.instance.ColumnX(column), 0, RelavtiveLineHandler.instance.TopRowZ);
        enemiesList.Add(enemy);
        enemy.OnSummon();
    }

    public void RemoveEnemy(EnemyBehavior enemy){
        enemiesList.Remove(enemy);
    }
#endregion

    public Slot GetSlot(int row, int column){
        return slots[row * girdSize.y + column];
    }
}
