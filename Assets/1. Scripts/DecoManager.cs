using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 이것저것 디자인적 요소들 관리하기 위한 클래스
/// </summary>
public class DecoManager : MonoBehaviour
{
    [SerializeField] private Iceberg icebergPrefab;
    [SerializeField] private Vector2 icebergSpawnInterval;
    [SerializeField] private int prewarmCount;

    private CancellationTokenSource icebergSpawnLoop;

    private void Start()
    {
        CombatManager.instance.onPhaseChange += OnPhaseChange;
        Prewarm();
    }

    private void OnDestroy(){
        if(CombatManager.instance != null) CombatManager.instance.onPhaseChange -= OnPhaseChange;
    }

    private void OnPhaseChange(CombatManager.Phase phase){
        if(phase == CombatManager.Phase.Combat){
            icebergSpawnLoop = new CancellationTokenSource();
            IcebergSpawnLoop(icebergSpawnLoop.Token).Forget();
        }else{
            if(icebergSpawnLoop != null){
                icebergSpawnLoop.Cancel();
                icebergSpawnLoop.Dispose();
                icebergSpawnLoop = null;
            }
        }
    }

    private void Prewarm(){
        for(int i = 0; i < prewarmCount; i++){
            Iceberg iceberg = GameObject.Instantiate(icebergPrefab);
            iceberg.transform.SetParent(RelavtiveLineHandler.instance.RandomDecoLine());
            iceberg.transform.localPosition = new Vector3(0, 0, UnityEngine.Random.Range(RelavtiveLineHandler.instance.BottomRowZ, RelavtiveLineHandler.instance.TopRowZ));
        }
    }

    private async UniTask IcebergSpawnLoop(CancellationToken ct){
        while(CombatManager.instance.phase == CombatManager.Phase.Combat){
            await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(icebergSpawnInterval.x, icebergSpawnInterval.y)), cancellationToken: ct);
            Iceberg iceberg = GameObject.Instantiate(icebergPrefab);
            iceberg.transform.SetParent(RelavtiveLineHandler.instance.RandomDecoLine());
            iceberg.transform.localPosition = new Vector3(0, 0, RelavtiveLineHandler.instance.TopRowZ);
        }
    }
}
