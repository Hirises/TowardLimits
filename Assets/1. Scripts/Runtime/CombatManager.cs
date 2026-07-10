using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 인게임 메니저
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Grid")]
    [LabelText("그리드 크기 Row, Column")]
    [SerializeField] public Vector2Int girdSize;
    [LabelText("슬롯 (좌상단에서 시작. hor->ver)")]
    [SerializeField] public Slot[] slots;

    [Header("Entities")]
    [SerializeField] public Transform enemySpawnRoot;
    [SerializeField] public Transform bulletRoot;
    [SerializeField] public Transform enemyBulletRoot;

    [Header("UI")]
    [SerializeField] public RectTransform mainCanvas;
    [SerializeField] public PlacementUI placementUIRoot;
    [SerializeField] public CombatUI combatUIRoot;
    [SerializeField] public TravelMap travelMap;
    [SerializeField] [TextArea] public string persuadeText;
    [SerializeField] public TMP_Text persuadeTextUI;
    [SerializeField] public RectTransform purchaseRoot;
    [SerializeField] public RectTransform purchaseArea;
    [SerializeField] public TMP_Text purchaseTextUI;
    [SerializeField] public Image Whiteout;
    [SerializeField] public SkillGage skillGage;
    [SerializeField] public Image HoldIcon;
    [SerializeField] public float slotSnapDistance = 2;

    [Header("다음 웨이브 시작 전 경고 표시 시간")]
    [SerializeField] private float nextWaveWarningLeadTime = 5f;

    [ReadOnly] private StageModel currentStage;
    [ReadOnly] public WaveModel currentWaveChart;
    [ReadOnly] private int currentWave = 0;
    [ReadOnly] private WaveModel preparedNextWaveChart;
    private float waveStartTime = 0;
    private List<EnemyBehavior> enemiesList = new List<EnemyBehavior>();
    private CancellationTokenSource enemySpawnLoop;
    private bool isDragging = false;
    private Action<Slot> endDrag;
    private Func<Slot, bool> condition;
    public event Action<Phase> onPhaseChange;
    [ReadOnly] private Phase _phase;
    public Phase phase {
        get => _phase;
        private set{
            if(value != _phase){
                _phase = value;
                onPhaseChange?.Invoke(value);
            }
        }
    }
    private bool isSummonEnded = false;
    private bool isNextWaveWarningShown = false;
    private bool IsCutscenePlaying => CutsceneManager.instance != null && CutsceneManager.instance.isPlaying;

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
            float elapsedTime = Time.time - waveStartTime;
            float ratio = elapsedTime / currentWaveChart.duration;
            ratio = Mathf.Clamp01(ratio);
            travelMap.UpdatePlayerPosition(currentWave, ratio);
            skillGage.UpdateGage();
            if(!isNextWaveWarningShown && currentWave + 1 < currentStage.waveCount && currentWaveChart.duration - elapsedTime <= nextWaveWarningLeadTime){
                ShowNextWaveWarningMark();
            }
            if(IsFinalWave() && isSummonEnded && enemiesList.Count == 0){
                NextCombatWave();
            }else if(ratio >= 1f){
                NextCombatWave();
            }
        }else if(phase == Phase.Purchase){
            if(Input.GetKeyDown(KeyCode.Space)){
                SkipPlacementPhase();
            }
            travelMap.UpdatePlayerPosition(currentWave, 0);
        }

        if(isDragging){
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas, Input.mousePosition, null, out var pos);
            HoldIcon.rectTransform.anchoredPosition = pos;
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
            Slot slot = RaycastSlot(Input.mousePosition, AllSlot);
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
        ClearPreparedNextWave();
        if(enemySpawnLoop != null){
            enemySpawnLoop.Cancel();
            enemySpawnLoop.Dispose();
            enemySpawnLoop = null;
        }
        foreach(Slot slot in slots){
            if(slot.unit != null){
                UnitBehavior unit = slot.unit;
                UnitStatus status = unit.status;
                GameManager.instance.playerData.units.Add(unit.status);
                unit.OnDisplacement();
                unit.Remove();
                status.RegularHeal(); 
            }
            slot.ShowBase(false);
            slot.ResetBuff();
            slot.unit = null;
        }
        foreach(Transform child in bulletRoot){
            Destroy(child.gameObject);
        }
        foreach(EnemyBehavior enemy in enemiesList){
            Destroy(enemy.gameObject);
        }
        enemiesList.Clear();
        phase = Phase.None;
        currentWaveChart = null;
        currentWave = 0;
        Whiteout.color = new Color(1, 1, 1, 0);
        RelavtiveLineHandler.instance.RowBase.transform.position = Vector3.zero;
        EndDrag();
        travelMap.Clear();
    }

    /// <summary>
    /// 게임을 초기화하고 준비합니다 (이미 모든 자원은 반환되어있는 빈 상태라고 가정)
    /// </summary>
    public void SetupGame(){
        placementUIRoot.Hide();
        if(GameManager.instance.currentStage == null){
            GameManager.instance.SetRandomStage(GameManager.instance.playerData.direction);
        }
        currentStage = GameManager.instance.currentStage;
        currentWave = -1;
        GameManager.instance.playerData.DT = 9999;
        combatUIRoot.UpdateDT();
        HoldIcon.gameObject.SetActive(false);

        travelMap.Initialize(currentStage.waveCount);
    }

    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame(){
        NextWave();
    }

    public void NextWave(){
        isSummonEnded = false;
        if(currentWaveChart != null){
            currentWaveChart.Unload();
        }
        currentWave++;
        if(currentWave >= currentStage.waveCount){
            EndCombatPhase();
            PurchasePhaseAnimation().Forget();
            return;
        }
        if(currentWave == 0){
            GameManager.instance.playerData.DT = 9999;
            combatUIRoot.UpdateDT();
        }
        StartPlacementPhase();

        if(GameManager.instance.playerData.stage == 0){
            if(currentWave == 0){
                CutsceneManager.instance.PlayCutScene("Combat1");
            }
        }
    }

    private void NextCombatWave(){
        bool hasPreparedNextWave = preparedNextWaveChart != null;
        bool isLastWave = currentWave + 1 == currentStage.waveCount;
        EndCombatPhase(hasPreparedNextWave, isLastWave);
        isSummonEnded = false;
        if(currentWaveChart != null && currentWaveChart != preparedNextWaveChart){
            currentWaveChart.Unload();
        }
        currentWave++;
        if(currentWave >= currentStage.waveCount){
            PurchasePhaseAnimation().Forget();
            return;
        }
        if(currentWave == 0){
            GameManager.instance.playerData.DT = 9999;
            combatUIRoot.UpdateDT();
        }
        if(preparedNextWaveChart != null){
            currentWaveChart = preparedNextWaveChart;
            preparedNextWaveChart = null;
        }else{
            currentWaveChart = ChooseRandomWaveChart(currentWave, GameManager.instance.playerData.direction, currentWave == currentStage.waveCount - 1);
            currentWaveChart.Load();
        }
        isNextWaveWarningShown = false;
        StartCombatPhase(currentWave == 1);

        if(GameManager.instance.playerData.stage == 0){
            if(currentWave == 0){
                CutsceneManager.instance.PlayCutScene("Combat1");
            }
        }
    }

    private bool IsFinalWave(){
        return currentWave == currentStage.waveCount - 1;
    }

    private void ShowNextWaveWarningMark(){
        isNextWaveWarningShown = true;
        preparedNextWaveChart = ChooseRandomWaveChart(currentWave + 1, GameManager.instance.playerData.direction, currentWave + 1 == currentStage.waveCount - 1);
        preparedNextWaveChart.Load();
        combatUIRoot.ShowWarningMark(preparedNextWaveChart);
        if(GameManager.instance.playerData.stage == 0 && currentWave + 1 == 1){
            CutsceneManager.instance.PlayCutScene("Combat2");
        }
    }

    private void ClearPreparedNextWave(){
        if(preparedNextWaveChart != null){
            preparedNextWaveChart.Unload();
            preparedNextWaveChart = null;
        }
        isNextWaveWarningShown = false;
        placementUIRoot.Hide();
    }

    public async UniTask PurchasePhaseAnimation(){
        foreach(Slot slot in slots){
            slot.ShowBase(false);
        }
        await DOTween.Sequence()
            .Append(RelavtiveLineHandler.instance.RowBase.transform.DOMoveZ(RelavtiveLineHandler.instance.TopRowZ, 3f).SetEase(Ease.InOutSine))
            .Insert(2, DOTween.To(() => Whiteout.color, x => Whiteout.color = x, new Color(1, 1, 1, 1), 1f).SetEase(Ease.InOutSine))
            .ToUniTask();
        UnlockUnits();
        StartPurchasePhase();
        RelavtiveLineHandler.instance.RowBase.transform.position = Vector3.zero;
        await DOTween.To(() => Whiteout.color, x => Whiteout.color = x, new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.InOutSine).ToUniTask();
        if(GameManager.instance.playerData.stage == 0){
            CutsceneManager.instance.PlayCutScene("Combat3");
            combatUIRoot.UpdateDT();
        }
    }

    public void SkipPlacementPhase(){
        if(phase == Phase.Placement){
            StartCombatPhase(currentWave == 1);
        }else if(phase == Phase.Purchase){
            ClearPhase();
            EndGame();
            StageClear();
        }
    }

    public void UnlockUnits()
    {
        // 유닛 해금
        // TODO: StageData로 옮기기
        switch(GameManager.instance.playerData.stage){
            case 0:
                UnlockUnit(UnitType.UnitX2);
                break;
            case 1:
                UnlockUnit(UnitType.UnitABS);
                UnlockUnit(UnitType.UnitC);
                break;
            case 2:
                UnlockUnit(UnitType.UnitX3);
                break;
        }
        // 1스테이지 클리어하면 절댓값 획득
        if(GameManager.instance.playerData.stage == 1 && !GameManager.instance.playerData.units.Any(status => status.unitType == UnitType.UnitABS)){
            GameManager.instance.playerData.units.Add(UnitStatus.FromType(UnitType.UnitABS));
        }
    }

    private void UnlockUnit(UnitType unitType)
    {
        if(unitType == UnitType.None){
            return;
        }
        if(GameManager.instance.playerData.unlockedUnits.Contains(unitType)){
            return;
        }
        GameManager.instance.playerData.unlockedUnits.Add(unitType);
    }

    public void StageClear(){
        GameManager.instance.playerData.stage++;
        GameManager.instance.playerData.Prove += currentStage.prove;
        GameManager.instance.SetGameSpeed(1f);
        if(GameManager.instance.playerData.Prove >= 100){
            LoadingScene.instance.ShowAndLoad("Clear", GameManager.instance.MIN_LOADING_DELAY).Forget();
        }else{
            LoadingScene.instance.ShowAndLoad("BaseCamp", GameManager.instance.MIN_LOADING_DELAY).Forget();
        }
    }

    /// <summary>
    /// 모든 유닛이 죽었는지 검사
    /// </summary>
    public void CheckGameOver(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                return;
            }
        }
        GameOver();
    }

    public void GameOver(){
        ClearBullets();
        GameManager.instance.SetGameSpeed(1f);
        LoadingScene.instance.ShowAndLoad("Clear", GameManager.instance.MIN_LOADING_DELAY).Forget();
    }
#endregion

#region Placement Phase
    public void StartPlacementPhase(){
        EndCombatPhase();
        phase = Phase.Placement;
        currentWaveChart = ChooseRandomWaveChart(currentWave, GameManager.instance.playerData.direction, currentWave == currentStage.waveCount - 1);
        currentWaveChart.Load();
        placementUIRoot.Show_Placement();
        combatUIRoot.Show_Placement();
    }

    public void EndPlacementPhase(){
        phase = Phase.None;
        EndDrag();
        placementUIRoot.Hide();
    }

    public Slot RaycastSlot(Vector3 screenPosition, Func<Slot, bool> condition){
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Slot slot = null;
        if(Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Slot", "Unit", "Ground"))){
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Slot")){
                var inner_slot = hit.collider.gameObject.GetComponentInParent<Slot>();
                if(condition(inner_slot)) return inner_slot;
            }else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit")){
                UnitBehavior unit = hit.collider.gameObject.GetComponentInParent<UnitBehavior>();
                var inner_slot = unit.slot;
                if(condition(inner_slot)) return inner_slot;
            }

            //가장 가까운 슬롯으로
            Vector3 point = hit.point;
            float minDistance = slotSnapDistance;
            foreach(var s in slots)
            {
                if(!condition(s)) continue;
                float distance = Vector3.Distance(s.transform.position, point);
                if(distance < minDistance)
                {
                    slot = s;
                    minDistance = distance;
                    continue;
                }
            }
        }
        return slot;
    }

    public UnitBehavior SummonUnit(UnitStatus status){
        UnitBehavior unit = Instantiate(status.model.unitBehavior, transform);
        unit.Initialize(status);
        return unit;
    }

    public void StartDrag(UnitIcon icon, UnitStatus status){
        if(IsCutscenePlaying || phase != Phase.Placement){
            return;
        }
        icon.SetAlpha(0.5f);
        condition = EmptySlot;
        endDrag = (slot) => {
            HoldIcon.gameObject.SetActive(false);
            icon.SetAlpha(1f);
            if(slot != null && slot.unit == null){
                if(GameManager.instance.playerData.DT > 0){
                    GameManager.instance.playerData.units.Remove(status);
                    placementUIRoot.UpdateUnit();
                    UnitBehavior unit = SummonUnit(status);
                    unit.OnPlacement(slot);
                    GameManager.instance.playerData.DT--;
                    combatUIRoot.UpdateDT();
                }else{
                    InsufficientDT();
                }
            }
        };
        Debug.Log($"StartDrag: {status.unitType}");
        isDragging = true;
        
        HoldIcon.sprite = status.model.holdMotion;
        HoldIcon.gameObject.SetActive(true);
    }

    public void StartDrag(UnitBehavior unit){
        if(IsCutscenePlaying || (phase != Phase.Placement && phase != Phase.Combat)){
            return;
        }
        condition = EmptySlot;
        endDrag = (slot) => {
            HoldIcon.gameObject.SetActive(false);
            unit.EndDrag();
            if(slot != null && slot.unit == null){
                if(GameManager.instance.playerData.DT > 0){
                    unit.OnDisplacement();
                    unit.OnPlacement(slot);
                    GameManager.instance.playerData.DT--;
                    combatUIRoot.UpdateDT();
                }else{
                    InsufficientDT();
                }
            }else if(phase == Phase.Placement && placementUIRoot.IsInInventoryArea(Input.mousePosition)){
                if(GameManager.instance.playerData.DT > 0){
                    unit.OnDisplacement();
                    GameManager.instance.playerData.units.Add(unit.status);
                    unit.Remove();
                    placementUIRoot.UpdateUnit();
                    GameManager.instance.playerData.DT--;
                    combatUIRoot.UpdateDT();
                } else{
                    InsufficientDT();
                }
            }
        };
        unit.StartDrag();
        Debug.Log($"StartDrag: {unit.unitType}");
        isDragging = true;
        GameManager.instance.SetGameSpeed(0.5f);
        
        HoldIcon.sprite = unit.status.model.holdMotion;
        HoldIcon.gameObject.SetActive(true);
    }

    public void EndDrag(){
        if(!isDragging){
            return;
        }
        if(condition == null) condition = AllSlot;
        Canvas canvas = mainCanvas.GetComponent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector3 offset = HoldIcon.rectTransform.rect.center * 0.25f;
        Vector2 pos = RectTransformUtility.WorldToScreenPoint(cam, HoldIcon.rectTransform.position + offset);
        Slot slot = RaycastSlot(pos, condition);
        endDrag?.Invoke(slot);
        isDragging = false;
        endDrag = null;
        Debug.Log($"EndDrag {slot} / {Input.mousePosition} {pos}");
        GameManager.instance.SetGameSpeed(combatUIRoot.GetGameSpeed());
    }

    private bool AllSlot(Slot slot)
    {
        return true;
    }

    private bool EmptySlot(Slot slot)
    {
        return slot.isEmpty;
    }

    public void InsufficientDT(){
        //나중에 시각적 VFX 보여줘서 'DT가 부족해서 못 옮기고 있다'라는 점을 명확히 보여주기
        Debug.Log("Insufficient DT");
    }
#endregion

#region Combat Phase
    public void StartCombatPhase(bool isFirstWave = false){
        EndPlacementPhase();
        if(currentWave == 0){
            GameManager.instance.playerData.DT = currentStage.DT;
            combatUIRoot.UpdateDT();
        }
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatStart();
            }
            slot.ShowBase(true);
        }
        phase = Phase.Combat;
        isSummonEnded = false;
        enemySpawnLoop = new CancellationTokenSource();
        WaveChartSpawnLoop(currentWaveChart, enemySpawnLoop.Token).Forget();
        if(isFirstWave){
            skillGage.SetGage(GameManager.instance.commonSettings.startSkillGage);
        }
        skillGage.Show();
        combatUIRoot.Show_Combat();
    }

    public void ClearBullets(){
        foreach(Transform child in bulletRoot){
            Destroy(child.gameObject);
        }
        foreach(Transform child in enemyBulletRoot){
            Destroy(child.gameObject);
        }
    }

    public void EndCombatPhase(bool preservePreparedNextWave = false, bool isLastWave = false){
        phase = Phase.None;
        if(enemySpawnLoop != null){
            enemySpawnLoop.Cancel();
            enemySpawnLoop.Dispose();
            enemySpawnLoop = null;
        }
        isSummonEnded = false;
        List<EnemyBehavior> copy = new List<EnemyBehavior>(enemiesList);
        foreach(EnemyBehavior enemy in copy){
            enemy.OnDeath();
        }
        enemiesList.Clear();
        if(isLastWave){
            ClearBullets();
        }
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnCombatEnd();
            }
            slot.ShowBase(true);
        }
        skillGage.Hide();
        if(!preservePreparedNextWave){
            ClearPreparedNextWave();
        }
    }

    public WaveModel ChooseRandomWaveChart(int difficulty, Polar polar, bool forFinalBoss){
        int stage = GameManager.instance.playerData.stage;
        WaveModel[] waveChart = DataFetcher.waveData.Where(w => w.difficulty.x <= difficulty && (w.difficulty.y < 0 || difficulty <= w.difficulty.y)  
            && (w.polar == polar || w.polar == Polar.Both || polar == Polar.Both) && w.forFinalBoss == forFinalBoss
            && w.stageRange.x <= stage && (w.stageRange.y < 0 || stage <= w.stageRange.y)).ToArray();
        if(waveChart.Length == 0){
            throw new Exception($"No wave chart found for difficulty: {difficulty}, polar: {polar}, forFinalBoss: {forFinalBoss}, stage: {stage}");
        }
        return waveChart[UnityEngine.Random.Range(0, waveChart.Length)];
    }
    
    private async UniTask WaveChartSpawnLoop(WaveModel waveChart, CancellationToken ct){
        float time = 0;
        waveStartTime = Time.time;
        foreach(var summon in waveChart.summonList){
            if(summon.startTime - time > 0.01f){
                await UniTask.Delay(TimeSpan.FromSeconds(summon.startTime - time), cancellationToken: ct);
            }
            time = summon.startTime;
            SummonEnemy(summon.enemyType, summon.lane);
        }
        isSummonEnded = true;
    }

    public void SummonEnemy(EnemyType enemyType, int column){
        SummonEnemy(enemyType, column, RelavtiveLineHandler.instance.TopRowZ);
    }

    public void SummonEnemy(EnemyType enemyType, int column, float zPos){
        EnemyBehavior enemy = Instantiate(enemyType.GetEnemyModel().enemyBehavior, enemySpawnRoot);
        enemy.transform.position = new Vector3(RelavtiveLineHandler.instance.ColumnX(column), 0, zPos);
        enemiesList.Add(enemy);
        enemy.OnSummon(column);
    }

    public void RemoveEnemy(EnemyBehavior enemy){
        enemiesList.Remove(enemy);
    }

    public void Persuade(int value){
        if(value == 0) return;
        GameManager.instance.playerData.Persuaded += value;
        persuadeTextUI.text = persuadeText.Replace("{0}", GameManager.instance.playerData.Persuaded.ToString());
        persuadeTextUI.gameObject.SetActive(true);
        CancelInvoke(nameof(HidePersuadeText));
        Invoke(nameof(HidePersuadeText), 1f);
        if(GameManager.instance.playerData.Persuaded >= 100){
            ClearPhase();
            EndGame();
            GameOver();
        }
    }

    private void HidePersuadeText(){
        persuadeTextUI.gameObject.SetActive(false);
    }
#endregion

#region Purchase Phase
    public void StartPurchasePhase(){
        phase = Phase.Purchase;
        GameManager.instance.playerData.DT += 1;
        combatUIRoot.UpdateDT();
        foreach(Slot slot in slots){
            if(slot.unit != null){
                UnitBehavior unit = slot.unit;
                GameManager.instance.playerData.units.Add(unit.status);
                unit.OnDisplacement();
                unit.Remove();
            }
            slot.ShowBase(false);
        }
        placementUIRoot.UpdateUnit();
        placementUIRoot.Show_Purchase();
        if(GameManager.instance.playerData.direction == Polar.North){
            purchaseTextUI.text = "Drop to Integrate";
        }else{
            purchaseTextUI.text = "Drop to derive";
        }
        purchaseRoot.gameObject.SetActive(true);
        combatUIRoot.Show_Purchase();
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
                    combatUIRoot.UpdateDT();
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
        UnitModel data = status.unitType.GetUnitModel();
        return data.integralTo.Length > 0 && AreCalculusResultsUnlocked(data.integralTo);
    }

    public bool IsDerivative(UnitStatus status){
        UnitModel data = status.unitType.GetUnitModel();
        return data.derivativeTo.Length > 0 && AreCalculusResultsUnlocked(data.derivativeTo);
    }

    private bool AreCalculusResultsUnlocked(CalculusResultElement[] results){
        List<UnitType> unlockedUnits = GameManager.instance.playerData.unlockedUnits;
        foreach(var element in results){
            if(element.unitType == UnitType.None){
                continue;
            }
            if(unlockedUnits == null || !unlockedUnits.Contains(element.unitType)){
                Debug.LogWarning($"{element.unitType}는 해금되지 않았습니다");
                return false;
            }
        }
        return true;
    }

    public void Integrate(UnitStatus status){
        UnitModel data = status.unitType.GetUnitModel();
        if(!IsIntegrable(status)){
            return;
        }
        GameManager.instance.playerData.DT--;
        int index = GameManager.instance.playerData.units.IndexOf(status);
        if(index == -1){
            return;
        }
        GameManager.instance.playerData.units.Remove(status);
        foreach(var element in data.integralTo){
            if(UnityEngine.Random.value > element.probability){
                continue;
            }
            for(int i = 0; i < element.amount; i++){
                GameManager.instance.playerData.units.Insert(index, UnitStatus.FromType(element.unitType));
            }
        }
    }

    public void Derivative(UnitStatus status){
        UnitModel data = status.unitType.GetUnitModel();
        if(!IsDerivative(status)){
            return;
        }
        GameManager.instance.playerData.DT--;
        int index = GameManager.instance.playerData.units.IndexOf(status);
        if(index == -1){
            return;
        }
        GameManager.instance.playerData.units.Remove(status);
        foreach(var element in data.derivativeTo){
            if(UnityEngine.Random.value > element.probability){
                continue;
            }
            for(int i = 0; i < element.amount; i++){
                if(element.unitType == UnitType.None) continue;
                GameManager.instance.playerData.units.Insert(index, UnitStatus.FromType(element.unitType));
            }
        }
    }

    public bool IsInPurchaseArea(Vector3 screenPosition){
        return RectTransformUtility.RectangleContainsScreenPoint(purchaseArea, screenPosition);
    }
#endregion
    
    public void ForcePerformSkill(int line){
        for(int i = 0; i < girdSize.x; i++){
            Slot slot = GetSlotAt(i, line);
            if(slot.unit != null){
                slot.unit.PerformSkill();
            }
        }
    }

    public Slot GetSlotAt(int row, int column){
        if(row < 0 || row >= girdSize.x || column < 0 || column >= girdSize.y){
            return null;
        }
        return slots[row * girdSize.y + column];
    }
}
