using System.Collections;
using UnityEngine;

public class DecoManager : MonoBehaviour
{
    [SerializeField] private Iceberg icebergPrefab;
    [SerializeField] private Vector2 icebergSpawnInterval;
    [SerializeField] private int prewarmCount;

    private Coroutine icebergSpawnLoop;

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
            icebergSpawnLoop = StartCoroutine(IcebergSpawnLoop());
        }else{
            if(icebergSpawnLoop != null) StopCoroutine(icebergSpawnLoop);
            icebergSpawnLoop = null;
        }
    }

    private void Prewarm(){
        for(int i = 0; i < prewarmCount; i++){
            Iceberg iceberg = GameObject.Instantiate(icebergPrefab);
            iceberg.transform.SetParent(RelavtiveLineHandler.instance.RandomDecoLine());
            iceberg.transform.localPosition = new Vector3(0, 0, UnityEngine.Random.Range(RelavtiveLineHandler.instance.BottomRowZ, RelavtiveLineHandler.instance.TopRowZ));
        }
    }

    private IEnumerator IcebergSpawnLoop(){
        while(CombatManager.instance.phase == CombatManager.Phase.Combat){
            yield return new WaitForSeconds(UnityEngine.Random.Range(icebergSpawnInterval.x, icebergSpawnInterval.y));
            Iceberg iceberg = GameObject.Instantiate(icebergPrefab);
            iceberg.transform.SetParent(RelavtiveLineHandler.instance.RandomDecoLine());
            iceberg.transform.localPosition = new Vector3(0, 0, RelavtiveLineHandler.instance.TopRowZ);
        }
    }
}
