using System.Collections;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 보스 출현 전 화면 전체에 띄우는 경고 오버레이. 게임 진행은 멈추지 않는다.
/// CombatManager.onPhaseChange 를 구독해서 전투 페이즈 시작 시 웨이브 차트의 보스 스폰 시각을 읽고 스스로 타이밍을 잡는다.
/// 타이머/트윈 모두 게임 시간(timeScale) 기준이라 배속, 컷신 일시정지와 스폰 타이밍이 어긋나지 않는다.
/// 이 오브젝트는 활성 상태로 둘 것 (보이기/숨기기는 CanvasGroup 알파로 처리).
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class BossWarningUI : MonoBehaviour
{
    [SerializeField] private Image vignette;
    [SerializeField] private TMP_Text warningText;

    [Header("타이밍")]
    [LabelText("경고 대상 적")]
    [SerializeField] private EnemyType[] bossTypes = { EnemyType.PolarBear, EnemyType.PolarBear_Captine };
    [LabelText("보스 스폰 몇 초 전에 경고할지")]
    [SerializeField, Min(0)] private float leadTime = 3f;
    [LabelText("최소 재생 시간 (보스가 웨이브 시작 직후 나올 때)")]
    [SerializeField, Min(0.5f)] private float minDuration = 1.5f;

    [Header("연출")]
    [SerializeField, Range(0, 1)] private float vignetteMaxAlpha = 0.6f;
    [SerializeField, Range(0, 1)] private float vignetteMinAlpha = 0.15f;
    [SerializeField, Range(0, 1)] private float textMinAlpha = 0.35f;
    [SerializeField, Min(1)] private int blinkCount = 3;
    [SerializeField, Min(0)] private float fadeTime = 0.25f;
    [SerializeField, Min(1)] private float textStartScale = 1.6f;

    private CanvasGroup canvasGroup;
    private Sequence sequence;
    private Coroutine scheduleRoutine;

    private void Awake(){
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Start(){
        CombatManager.instance.onPhaseChange += OnPhaseChange;
    }

    private void OnDestroy(){
        if(CombatManager.instance != null) CombatManager.instance.onPhaseChange -= OnPhaseChange;
    }

    private void OnPhaseChange(CombatManager.Phase phase){
        Stop();
        if(phase == CombatManager.Phase.Combat){
            // 전투 페이즈 전환과 같은 프레임에 스폰 루프가 시작되므로 지금을 웨이브 시작 시각으로 본다
            scheduleRoutine = StartCoroutine(ScheduleWarnings(CombatManager.instance.currentWaveChart));
        }
    }

    private IEnumerator ScheduleWarnings(WaveModel waveChart){
        if(waveChart?.summonList == null) yield break;
        float[] bossSpawnTimes = waveChart.summonList
            .Where(s => bossTypes.Contains(s.enemyType))
            .Select(s => s.startTime)
            .ToArray(); // summonList 는 startTime 오름차순 정렬되어 있음

        float elapsed = 0;
        float lastWarningTime = float.NegativeInfinity;
        foreach(float spawnTime in bossSpawnTimes){
            float warningTime = Mathf.Max(0, spawnTime - leadTime);
            if(warningTime - lastWarningTime < leadTime) continue; // 연달아 나오는 보스는 경고 한 번만
            lastWarningTime = warningTime;

            if(warningTime > elapsed){
                yield return new WaitForSeconds(warningTime - elapsed);
                elapsed = warningTime;
            }
            Play(spawnTime - warningTime);
        }
    }

    /// <param name="duration">보스 스폰까지 남은 시간(게임 시간 기준)</param>
    [Button]
    public void Play(float duration = 3f){
        sequence?.Kill();

        duration = Mathf.Max(duration, minDuration, fadeTime * 2 + 0.1f);
        float halfBlink = (duration - fadeTime * 2) / blinkCount / 2f;

        canvasGroup.alpha = 0;
        SetAlpha(vignette, 0);
        SetAlpha(warningText, 1);
        warningText.rectTransform.localScale = Vector3.one * textStartScale;

        sequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, fadeTime))
            .Join(warningText.rectTransform.DOScale(1, fadeTime).SetEase(Ease.OutBack));
        for(int i = 0; i < blinkCount; i++){
            sequence
                .Append(vignette.DOFade(vignetteMaxAlpha, halfBlink).SetEase(Ease.OutSine))
                .Join(warningText.DOFade(1, halfBlink))
                .Append(vignette.DOFade(vignetteMinAlpha, halfBlink).SetEase(Ease.InSine))
                .Join(warningText.DOFade(textMinAlpha, halfBlink));
        }
        sequence
            .Append(canvasGroup.DOFade(0, fadeTime))
            .SetLink(gameObject);
    }

    /// <summary>예약된 경고를 취소하고 재생 중인 경고를 즉시 숨긴다</summary>
    public void Stop(){
        if(scheduleRoutine != null){
            StopCoroutine(scheduleRoutine);
            scheduleRoutine = null;
        }
        sequence?.Kill();
        sequence = null;
        canvasGroup.alpha = 0;
    }

    private static void SetAlpha(Graphic graphic, float alpha){
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }
}
