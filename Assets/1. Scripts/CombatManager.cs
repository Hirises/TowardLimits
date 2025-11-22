using UnityEngine;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] public TravelMap travelMap;
    [SerializeField] [TextArea] public string persuadeText;
    [SerializeField] public TMP_Text persuadeTextUI;
    [SerializeField] public RectTransform purchaseRoot;
    [SerializeField] public TMP_Text purchaseTextUI;

    [ReadOnly] private StageData currentStage;
    [ReadOnly] private WaveChart currentWaveChart;
    [ReadOnly] private int currentWave = 0;
    private float waveStartTime = 0;
    private List<EnemyBehavior> enemiesList = new List<EnemyBehavior>();
    private Coroutine enemySpawnLoop;
    private bool isDragging = false;
    private Action<Slot> endDrag;

    [ReadOnly] public Phase phase;
    private bool isSummonEnded = false;

    public enum Phase {
        None,       //초기 상태
        Placement,  //배치 단계
        Combat,     //전투 단계
        Purchase,   //구매 단계
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
                SkipPlacementPhase();
            }
            travelMap.UpdatePlayerPosition(currentWave, 0);
        }else if(phase == Phase.Combat){
            if(isSummonEnded && enemiesList.Count == 0){
                NextWave();
            }
            float ratio = (Time.time - waveStartTime) / currentWaveChart.duration;
            ratio = Mathf.Clamp01(ratio);
            travelMap.UpdatePlayerPosition(currentWave, ratio);
        }else if(phase == Phase.Purchase){
            if(Input.GetKeyDown(KeyCode.Space)){
                SkipPlacementPhase();
            }
            travelMap.UpdatePlayerPosition(currentWave, 0);
        }

        if(isDragging){
            if(Input.GetMouseButtonUp(0)){
                EndDrag();
            }
        }

        if(GameManager.instance.DEBUG_MODE){
            TestKey();
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

    public void ClearPhase(){
        if(phase == Phase.Placement){
            EndPlacementPhase();
        }else if(phase == Phase.Combat){
            EndCombatPhase();
        }else if(phase == Phase.Purchase){
            EndPurchasePhase();
        }
    }

    /// <summary>
    /// 게임을 종료하고 자원을 반환합니다. phase가 none일때 호출해야합니다.
    /// </summary>
    public void EndGame(){
        if(enemySpawnLoop != null) StopCoroutine(enemySpawnLoop);
        enemySpawnLoop = null;
        foreach(Slot slot in slots){
            if(slot.unit != null){
                UnitBehavior unit = slot.unit;
                GameManager.instance.playerData.units.Add(unit.status);
                unit.OnDisplacement();
                unit.Remove();
            }
            slot.ShowBase(false);
            slot.unit = null;
        }
        foreach(EnemyBehavior enemy in enemiesList){
            Destroy(enemy.gameObject);
        }
        enemiesList.Clear();
        phase = Phase.None;
        EndDrag();
        travelMap.Clear();
    }

    /// <summary>
    /// 게임을 초기화하고 준비합니다 (이미 모든 자원은 반환되어있는 빈 상태라고 가정)
    /// </summary>
    public void SetupGame(){
        placementUIRoot.Hide();
        currentStage = GameManager.instance.currentStage;
        currentWave = -1;
        GameManager.instance.playerData.DT = 999;
        travelMap.Initialize(currentStage.waveCount);
        GameManager.instance.playerData.Persuaded = 0;
    }

    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame(){
        NextWave();
    }

    public void NextWave(){
        isSummonEnded = false;
        currentWave++;
        if(currentWave >= currentStage.waveCount){
            EndCombatPhase();
            StartPurchasePhase();
            return;
        }
        if(currentWave == 0){
            GameManager.instance.playerData.DT = 999;
        }else if(currentWave == 1){
            GameManager.instance.playerData.DT = currentStage.DT;
        }
        StartPlacementPhase();
    }

    public void SkipPlacementPhase(){
        if(phase == Phase.Placement){
            StartCombatPhase();
        }else if(phase == Phase.Purchase){
            ClearPhase();
            EndGame();
            GoToBaseCamp();
        }
    }
#endregion

#region Placement Phase
    public void StartPlacementPhase(){
        EndCombatPhase();
        phase = Phase.Placement;
        placementUIRoot.Show_Placement();
    }

    public void EndPlacementPhase(){
        phase = Phase.None;
        EndDrag();
        placementUIRoot.Hide();
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

    public UnitBehavior SummonUnit(UnitStatus status){
        UnitBehavior unit = Instantiate(unitMap[status.unitType], transform);
        unit.Initialize(status);
        return unit;
    }

    public void StartDrag(UnitIcon icon, UnitStatus status){
        if(phase != Phase.Placement){
            return;
        }
        icon.SetAlpha(0.5f);
        endDrag = (slot) => {
            icon.SetAlpha(1f);
            if(slot != null && slot.unit == null){
                if(GameManager.instance.playerData.DT > 0){
                    GameManager.instance.playerData.units.Remove(status);
                    placementUIRoot.UpdateUnit();
                    UnitBehavior unit = SummonUnit(status);
                    unit.OnPlacement(slot);
                    GameManager.instance.playerData.DT--;
                    placementUIRoot.UpdateDT();
                }else{
                    InsufficientDT();
                }
            }
        };
        Debug.Log($"StartDrag: {status.unitType}");
        isDragging = true;
    }

    public void StartDrag(UnitBehavior unit){
        if(phase != Phase.Placement){
            return;
        }
        endDrag = (slot) => {
            unit.EndDrag();
            if(slot != null && slot.unit == null){
                if(GameManager.instance.playerData.DT > 0){
                    unit.OnDisplacement();
                    unit.OnPlacement(slot);
                    GameManager.instance.playerData.DT--;
                    placementUIRoot.UpdateDT();
                }else{
                    InsufficientDT();
                }
            }else if(placementUIRoot.IsInInventoryArea(Input.mousePosition)){
                if(GameManager.instance.playerData.DT > 0){
                    unit.OnDisplacement();
                    GameManager.instance.playerData.units.Add(unit.status);
                    unit.Remove();
                    placementUIRoot.UpdateUnit();
                    GameManager.instance.playerData.DT--;
                    placementUIRoot.UpdateDT();
                } else{
                    InsufficientDT();
                }
            }
        };
        unit.StartDrag();
        Debug.Log($"StartDrag: {unit.unitType}");
        isDragging = true;
    }

    public void EndDrag(){
        if(!isDragging){
            return;
        }
        Slot slot = RaycastSlot(Input.mousePosition);
            endDrag?.Invoke(slot);
        isDragging = false;
        endDrag = null;
        Debug.Log($"EndDrag {slot}");
    }

    public void GoToBaseCamp(){
        LoadingScene.instance.ShowAndLoad("BaseCamp", GameManager.instance.MIN_LOADING_DELAY);
    }

    public void InsufficientDT(){
        Debug.Log("Insufficient DT");
    }
#endregion

#region Combat Phase
    public void StartCombatPhase(){
        EndPlacementPhase();
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatStart();
            }
            slot.ShowBase(false);
        }
        phase = Phase.Combat;
        isSummonEnded = false;
        currentWaveChart = ChooseRandomWaveChart(1, Polar.Both);
        if(currentWaveChart == null){
            enemySpawnLoop = StartCoroutine(SimpleEnemySpawnLoop());
            return;
        }
        Debug.Log($"ChooseWaveChart: {currentWaveChart.filePath}");
        currentWaveChart.Load();
        enemySpawnLoop = StartCoroutine(WaveChartSpawnLoop(currentWaveChart));
    }

    public void EndCombatPhase(){
        phase = Phase.None;
        if(enemySpawnLoop != null) StopCoroutine(enemySpawnLoop);
        enemySpawnLoop = null;
        isSummonEnded = false;
        List<EnemyBehavior> copy = new List<EnemyBehavior>(enemiesList);
        foreach(EnemyBehavior enemy in copy){
            enemy.OnDeath();
        }
        enemiesList.Clear();
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatEnd();
            }
            slot.ShowBase(true);
        }
    }

    public WaveChart ChooseRandomWaveChart(int difficulty, Polar polar){
        WaveChart[] waveChart = waveCharts.Where(w => w.difficulty.x <= difficulty && (w.difficulty.y < 0 || difficulty <= w.difficulty.y)  
            && (w.polar == polar || w.polar == Polar.Both)).ToArray();
        if(waveChart.Length == 0){
            return null;
        }
        return waveChart[UnityEngine.Random.Range(0, waveChart.Length)];
    }
    
    private IEnumerator WaveChartSpawnLoop(WaveChart waveChart){
        float time = 0;
        waveStartTime = Time.time;
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
            SummonEnemy(EnemyType.EIRNormal, UnityEngine.Random.Range(0, girdSize.y));
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

    public void Persuade(int value){
        GameManager.instance.playerData.Persuaded += value;
        persuadeTextUI.text = persuadeText.Replace("{0}", GameManager.instance.playerData.Persuaded.ToString());
        persuadeTextUI.gameObject.SetActive(true);
        CancelInvoke(nameof(HidePersuadeText));
        Invoke(nameof(HidePersuadeText), 1f);
        if(GameManager.instance.playerData.Persuaded >= 100){
            ClearPhase();
            EndGame();
            GoToBaseCamp();
        }
    }

    private void HidePersuadeText(){
        persuadeTextUI.gameObject.SetActive(false);
    }
#endregion

#region Purchase Phase
    public void StartPurchasePhase(){
        phase = Phase.Purchase;
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnDisplacement();
                GameManager.instance.playerData.units.Add(slot.unit.status);
                slot.unit.Remove();
            }
            slot.ShowBase(false);
        }
        placementUIRoot.UpdateUnit();
        placementUIRoot.Show_Purchase();
        if(GameManager.instance.playerData.direction == Polar.North){
            purchaseTextUI.text = "Drag Here to Integrate";
        }else{
            purchaseTextUI.text = "Drag Here to Derivative";
        }
        purchaseRoot.gameObject.SetActive(true);
    }

    public void EndPurchasePhase(){
        phase = Phase.None;
        placementUIRoot.Hide();
        purchaseRoot.gameObject.SetActive(false);
    }

    public void StartDrag_Purchase(UnitIcon icon, UnitStatus status){
        if(phase != Phase.Purchase){
            return;
        }
        icon.SetAlpha(0.5f);
        endDrag = (slot) => {
            icon.SetAlpha(1f);
            if(IsInPurchaseArea(Input.mousePosition)){
                if(GameManager.instance.playerData.direction == Polar.North && !IsIntegrable(status)){
                    return;
                }
                if(GameManager.instance.playerData.direction == Polar.South && !IsDerivative(status)){
                    return;
                }

                if(GameManager.instance.playerData.DT > 0){
                    if(GameManager.instance.playerData.direction == Polar.North){
                        Integrate(status);
                    }else{
                        Derivative(status);
                    }
                    GameManager.instance.playerData.DT--;
                    placementUIRoot.UpdateDT();
                    placementUIRoot.UpdateUnit();
                }else{
                    InsufficientDT();
                }
            }
        };
        Debug.Log($"StartDrag: {status.unitType}");
        isDragging = true;
    }

    public bool IsIntegrable(UnitStatus status){
        UnitData data = status.unitType.GetUnitData();
        return data.integralTo != UnitType.None;
    }

    public bool IsDerivative(UnitStatus status){
        UnitData data = status.unitType.GetUnitData();
        return data.derivativeTo != UnitType.None;
    }

    public void Integrate(UnitStatus status){
        UnitData data = status.unitType.GetUnitData();
        if(!IsIntegrable(status)){
            return;
        }
        int index = GameManager.instance.playerData.units.IndexOf(status);
        if(index == -1){
            return;
        }
        GameManager.instance.playerData.units.Remove(status);
        GameManager.instance.playerData.units.Insert(index, UnitStatus.FromType(data.integralTo));
    }

    public void Derivative(UnitStatus status){
        UnitData data = status.unitType.GetUnitData();
        if(!IsDerivative(status)){
            return;
        }
        int index = GameManager.instance.playerData.units.IndexOf(status);
        if(index == -1){
            return;
        }
        GameManager.instance.playerData.units.Remove(status);
        for(int i = 0; i < data.derivativeAmount; i++){
            GameManager.instance.playerData.units.Insert(index, UnitStatus.FromType(data.derivativeTo));
        }
    }

    public bool IsInPurchaseArea(Vector3 screenPosition){
        return RectTransformUtility.RectangleContainsScreenPoint(purchaseRoot, screenPosition);
    }
#endregion

    public Slot GetSlot(int row, int column){
        return slots[row * girdSize.y + column];
    }
}
